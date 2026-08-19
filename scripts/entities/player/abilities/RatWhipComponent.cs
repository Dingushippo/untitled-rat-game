using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public sealed class RatWhipComponent
{
    private const float RAT_LENGTH = 0.7f;
    private const float MIN_TAIL_EXTENT = 0.2f;
    private const float MAX_TAIL_EXTENT = 0.5f;

    // private const float RAT_LENGTH
    private Hand _hand;

    public RatWhipComponent(Hand hand)
    {
        _hand = hand;
    }

    private bool TryGetTargetAnchorPoint()
    {
        throw new NotImplementedException();
    }

    private Vector3[] GetWhipNodePositions(Vector3 anchorPos)
    {
        /*
        Get list of points from current hand position, to target point.
        Point I = rat, point I+1 = ConeTwistJoint
        2n-1 points; n rats, n-1 joints
        */

        Vector3 startPos = _hand.GlobalPosition;
        float whipLength = startPos.DistanceTo(anchorPos);
        int numSegmentsBase = Mathf.FloorToInt(whipLength / (RAT_LENGTH + MIN_TAIL_EXTENT));
        float actualSegmentLength = whipLength / (2 * numSegmentsBase - 1);

        return FloatRange(0, 1, 1 / numSegmentsBase)
            .Select(t => startPos.Lerp(anchorPos, t))
            .ToArray();
    }

    private static IEnumerable<float> FloatRange(float min, float max, float step)
    {
        for (float value = min; value <= max; value += step)
        {
            yield return value;
        }
    }
}
