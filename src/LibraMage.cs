using LibraMage.Entities;
using LibraMage.Results;
using UnityEngine;

public class LibraMageUtils
{
    public static TangentCircleResult GetCircleTangentToTwoLines(Vector2 intersectionPoint, Vector2 firstLineUniquePoint, Vector2 secondLineUniquePoint,
                                                        float circleRadius)
    {
        Vector2 intersectionToFirstLinePt = firstLineUniquePoint - intersectionPoint;
        intersectionToFirstLinePt.Normalize();
        Vector2 intersectionToSecondLinePt = secondLineUniquePoint - intersectionPoint;
        intersectionToSecondLinePt.Normalize();
        Vector2 intersectionToCircleCenter = (intersectionToFirstLinePt + intersectionToSecondLinePt) / 2;
        intersectionToCircleCenter.Normalize();

        float alpha = Mathf.Acos((intersectionToFirstLinePt.x * intersectionToSecondLinePt.x + intersectionToFirstLinePt.y * intersectionToSecondLinePt.y)
            / (intersectionToSecondLinePt.magnitude * intersectionToFirstLinePt.magnitude)) / 2;

        float d = circleRadius / Mathf.Tan(alpha);
        d = Mathf.Abs(d);

        intersectionToFirstLinePt *= d;
        intersectionToSecondLinePt *= d;

        firstLineUniquePoint = intersectionPoint + intersectionToFirstLinePt;
        secondLineUniquePoint = intersectionPoint + intersectionToSecondLinePt;

        float b = circleRadius / Mathf.Sin(alpha);
        b = Mathf.Abs(b);
        intersectionToCircleCenter *= b;

        Circle tangentCirle = new Circle(intersectionToCircleCenter + intersectionPoint, circleRadius);

        return new TangentCircleResult(tangentCirle, firstLineUniquePoint, secondLineUniquePoint);
    }
}