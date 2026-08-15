using Godot;
using Godot.Collections;
using System;

public partial class ThrowPreview : Node3D
{
    private const int STRIDE = 16; // 12 transform floats, 4 custom data floats
    private ThrowPreviewTuning _tuning;
    [Export]
    public ThrowPreviewTuning Tuning
    {
        get => _tuning;
        set
        {
            _tuning = value;
            _tuning.EmitChanged();
            if (_reticleMaterial != null && _dotMaterial != null)
                PushTuningToShader();
        }
    }
    [Export] public bool LiveTuning = false;
    [Export] public Shader DotShader;
    [Export] public Shader ReticleShader;
    [Export] public int MaxDots = 300;
    private MultiMeshInstance3D _dots;
    private MeshInstance3D _reticle;
    private Mesh _reticleMesh;
    private MultiMesh _multiMesh;
    private ShaderMaterial _dotMaterial;
    private ShaderMaterial _reticleMaterial;
    private float[] _buffer;
    private int _count;
    private Aabb _bounds;
    private Camera3D _camera;
    private Color _lastColor = new(0, 0, 0, 0);
    private bool _visible;

    public override void _Ready()
    {
        TopLevel = true;
        GlobalTransform = Transform3D.Identity;

        _dotMaterial = new ShaderMaterial { Shader = DotShader };
        _reticleMaterial = new ShaderMaterial { Shader = ReticleShader };

        PushTuningToShader();

        _multiMesh = new MultiMesh
        {
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
            UseColors = false,
            UseCustomData = true,
            Mesh = new QuadMesh { Size = Vector2.One },
            InstanceCount = MaxDots,
        };
        _multiMesh.VisibleInstanceCount = 0;

        _dots = new MultiMeshInstance3D
        {
            Multimesh = _multiMesh,
            MaterialOverride = _dotMaterial,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            GIMode = GeometryInstance3D.GIModeEnum.Disabled
        };
        AddChild(_dots);

        _reticleMesh = new PlaneMesh { Size = Vector2.One };
        _reticle = new MeshInstance3D
        {
            Mesh = _reticleMesh,
            MaterialOverride = _reticleMaterial,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            GIMode = GeometryInstance3D.GIModeEnum.Disabled
        };
        AddChild(_reticle);

        _buffer = new float[MaxDots * STRIDE];
    }

    private void PushTuningToShader()
    {
        _dotMaterial.SetShaderParameter("bounce_falloff", Tuning.BounceFalloff);
        _dotMaterial.SetShaderParameter("fade_start", Tuning.FadeStart);
        _dotMaterial.SetShaderParameter("fade_end", Tuning.FadeEnd);
        _dotMaterial.SetShaderParameter("min_alpha", Tuning.MinAlpha);
        _dotMaterial.SetShaderParameter("flow_speed", Tuning.FlowSpeed);
        _dotMaterial.SetShaderParameter("flow_frequency", Tuning.FlowFrequency);
        _dotMaterial.SetShaderParameter("flow_strength", Tuning.FlowStrength);
        _dotMaterial.SetShaderParameter("impact_scale", Tuning.ImpactRingSize);

        _reticleMaterial.SetShaderParameter("pulse_rate", Tuning.ReticlePulseRate);
        _reticleMaterial.SetShaderParameter("spin_rate", Tuning.ReticleSpinRate);
        _reticleMaterial.SetShaderParameter("chevron_rate", Tuning.ReticleChevronSpeed);


    }

    private void Emit(Vector3 world, float metres, float total, int segment, bool impact)
    {
        if (_count >= MaxDots) return;

        float distance = _camera.GlobalPosition.DistanceTo(world);
        float t = Mathf.Clamp(Mathf.InverseLerp(Tuning.FadeStart, Tuning.FadeEnd, distance), 0f, 1f);
        float size = Mathf.Lerp(Tuning.DotSize, Tuning.DotSize * Tuning.NearFarSizeRatio, t);

        int o = _count * STRIDE;
        // Transform
        _buffer[o + 0] = size;
        _buffer[o + 1] = 0f;
        _buffer[o + 2] = 0f;
        _buffer[o + 3] = world.X;
        _buffer[o + 4] = 0f;
        _buffer[o + 5] = size;
        _buffer[o + 6] = 0f;
        _buffer[o + 7] = world.Y;
        _buffer[o + 8] = 0f;
        _buffer[o + 9] = 0f;
        _buffer[o + 10] = size;
        _buffer[o + 11] = world.Z;

        // Custom data
        _buffer[o + 12] = total > 0.0001f ? metres / total : 0f;
        _buffer[o + 13] = segment;
        _buffer[o + 14] = impact ? 1f : 0f;
        _buffer[o + 15] = metres;

        _count++;
    }

    public void Resample(in ThrowPath path)
    {
        if (LiveTuning)
            PushTuningToShader();
        _count = 0;

        Vector3[] points = path.Points;
        if (points.Length < 2) return;

        float total = path.Length;
        float cumulative = 0f;
        float carry = 0f;

        int impactCursor = 0;

        Vector3 min = points[0], max = points[0];
        Emit(points[0], 0f, total, path.Segments[0], false);

        for (int i = 1; i < points.Length; i++)
        {
            Vector3 a = points[i - 1];
            Vector3 b = points[i];
            float edge = a.DistanceTo(b);

            if (edge > 0.0001f)
            {
                for (float d = Tuning.DotSpacing - carry; d <= edge; d += Tuning.DotSpacing)
                {
                    Vector3 p = a.Lerp(b, d / edge);
                    Emit(p, cumulative + d, total, path.Segments[i], impact: false);
                    min = min.Min(p); max = max.Max(p);
                }

                carry = (carry + edge) % Tuning.DotSpacing;
                cumulative += edge;
            }

            if (impactCursor < path.Impacts.Length && path.Impacts[impactCursor] == i)
            {
                Emit(b, cumulative, total, path.Segments[i], impact: true);
                min = min.Min(b); max = max.Max(b);
                carry = 0f;
                impactCursor++;
            }
        }

        _bounds = new Aabb(min, max - min).Grow(Tuning.DotSize * Tuning.NearFarSizeRatio);
    }
    public void ShowPreview(in ThrowPath path)
    {
        _camera ??= GetViewport().GetCamera3D();

        if (_camera is null) return;

        if (!_visible)
        {
            _visible = true;
        }

        Color color = PathColor(path);
        using (Profiler.Sample("throw.preview"))
        {
            Resample(path);
            if (color != _lastColor)
            {
                _dotMaterial.SetShaderParameter("trail_color", color);
                _reticleMaterial.SetShaderParameter("reticle_color", color);
                _lastColor = color;
                SetReticleGlyphMode(path);
            }

            _multiMesh.CustomAabb = _bounds;
            _multiMesh.Buffer = _buffer;
            _multiMesh.VisibleInstanceCount = _count;

            SetReticle(path);
            _dots.Visible = _count > 0;
            _reticle.Visible = _dots.Visible;
        }

    }

    public void HidePreview()
    {
        if (!_visible)
        {
            return;
        }
        _visible = false;
        _multiMesh.VisibleInstanceCount = 00;
        _dots.Visible = false;
        _reticle.Visible = false;
    }

    private void SetReticleGlyphMode(ThrowPath path)
    {
        if (path.ThrowTarget.IsIntake) _reticleMaterial.SetShaderParameter("glyph_mode", 2);
        else if (path.Homing) _reticleMaterial.SetShaderParameter("glyph_mode", 1);
        else _reticleMaterial.SetShaderParameter("glyph_mode", 0);
    }

    private Color PathColor(ThrowPath path)
    {
        if (path.ThrowTarget.IsIntake) return Tuning.IntakeThrowColor;
        else if (path.Homing) return Tuning.SlotThrowColor;
        else return Tuning.FreeThrowColor;
    }

    public void SetReticle(ThrowPath path)
    {
        Vector3 reticlePos = path.End + Vector3.Up * 0.01f;
        Vector3 targetRaycastPos = reticlePos + Vector3.Down;
        if (RaycastUtils.Ray(this, reticlePos, targetRaycastPos, out Dictionary result, PhysicsLayers.WORLD))
        {
            Vector3 hitNormal = result["normal"].AsVector3();
            Vector3 hitPosition = result["position"].AsVector3();

            Vector3 up = hitNormal;
            Vector3 seed = Math.Abs(up.Dot(Vector3.Up)) > 0.99f ? Vector3.Forward : Vector3.Up;
            Vector3 right = seed.Cross(up).Normalized();
            Vector3 forward = up.Cross(right);
            Basis basis = new(right, up, forward);

            _reticle.GlobalTransform = new Transform3D(
                basis.Scaled(Vector3.One * Tuning.ReticleSize),
                hitNormal * Tuning.SurfaceOffset + hitPosition
            );
        }
    }
}