# LibraMage
A multi-purpose math and physics library for Unity3D.

# Documentation
```cs
public static TangentCircleResult GetCircleTangentToTwoLines(Vector2 intersectionPoint, Vector2 firstLineUniquePoint, Vector2 secondLineUniquePoint, float circleRadius)
```
This method gets you a circle lodged in between two intersecting lines.


```cs
public static GameObject CreateTrajectoryRenderer(GameObject parent)
```
This method gives you a GameObject that can render trajectories.

# Examples
The following are some examples of where and how I use LibraMage in my projects.
#### Fat Bunny: Rounded Corners
![Fat Bunny](img/GetTangentCircleExample.png)

In Fat Bunny, the lines of the mountainous terrain have rounded corners at the places where they intersect. Those rounded corners are actually part of a circle that is tangent to the two lines. To get that circle (and subsequently, to generate the rounded corner), use the GetCircleTangentToTwoLines method.


#### Fat Bunny: Trajectory Prediction
![Fat Bunny](img/TrajectoryPredictionExample.png)

In Fat Bunny, one feature we've decided to experiment with is trajectory rendering. It shows you the path your bunny will follow gracefully in the air after he jumps. To get such a trajectory renderer, use the CreateTrajectoryRenderer method.