using Godot;

public class Pid3D
{
    private Vector3 _prevError;
    private Vector3 _errorIntegral;

    public Vector3 Update(PidTuning tuning, Vector3 error, float delta)
    {
        _errorIntegral += error * delta;
        Vector3 errorDerivative = (error - _prevError) / delta;
        _prevError = error;
        return tuning.P * error + tuning.I * _errorIntegral + tuning.D * errorDerivative;
    }
}
