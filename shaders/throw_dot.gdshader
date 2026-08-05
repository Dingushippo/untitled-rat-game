shader_type spatial;
render_mode unshaded, blend_add, depth_draw_never, cull_disabled;

uniform vec4  trail_color : source_color;
uniform float bounce_falloff  = 0.6;
uniform float fade_start      = 6.0;
uniform float fade_end        = 30.0;
uniform float min_alpha       = 0.15;
uniform float flow_speed      = 4.0;
uniform float flow_frequency  = 1.5;
uniform float flow_strength   = 0.5;
uniform float impact_scale    = 2.5;

varying flat vec4 custom;

void vertex() {
    custom = INSTANCE_CUSTOM;

    // Billboard: drop the rotation from the modelview, keep translation and scale.
    float s = length(MODEL_MATRIX[0].xyz) * mix(1.0, impact_scale, custom.z);
    MODELVIEW_MATRIX = VIEW_MATRIX * mat4(
        INV_VIEW_MATRIX[0] * s,
        INV_VIEW_MATRIX[1] * s,
        INV_VIEW_MATRIX[2] * s,
        MODEL_MATRIX[3]);
}

void fragment() {
    float d = length(UV - vec2(0.5));

    // Disc for a trail dot, ring for an impact marker.
    float disc = 1.0 - smoothstep(0.35, 0.5, d);
    float ring = (1.0 - smoothstep(0.42, 0.5, d)) * smoothstep(0.28, 0.36, d);
    float mask = mix(disc, ring, custom.z);
    if (mask < 0.01) discard;

    float bounce = pow(bounce_falloff, custom.y);
    float depth  = 1.0 - smoothstep(fade_start, fade_end, length(VERTEX));
    float flow   = 1.0 + flow_strength * sin(custom.w * flow_frequency - TIME * flow_speed);
	ALBEDO  = trail_color.rgb;
    ALPHA = mask * trail_color.a * bounce * max(depth, min_alpha);
	if (custom.z == 0.0) {
		ALBEDO *= flow;
		ALPHA *= flow;
	}
}