# LibraMage
A multi-purpose math and physics library for Unity.

# Nomenclature
LibraMage also serves as a tribute to Final Fantasy, one of my favorite video game series. It is generally themed after it, and some of the class names have that FF ring to them, so kudos if you notice and feel the influence!

# Documentation
LibraMage has two entry points for its functionality: the classes Mathga and Physicsga. Both classes are static and, in conjunction, act as LibraMage's main interface. Mathga exposes the math methods, while Physicsga exposes the physics ones.

## Mathga
#### GetCircleTangentToTwoLines
```cs
TangentCircleResult GetCircleTangentToTwoLines(Vector2 intersectionPoint, Vector2 firstLineUniquePoint, Vector2 secondLineUniquePoint,
                                                            float circleRadius)
```
GetCircleTangentToTwoLines returns a circle that is tangent to any two non-parallel lines.

* ```intersectionPoint (Vector2)``` The point where the two lines intersect.

* ```firstLineUniquePoint (Vector2)``` Any point that lies on the first line and not on the second line.

* ```secondLineUniquePoint (Vector2)``` Any point that lies on the second line and not on the first line.

* ```circleRadius (float)``` The radius of the desired circle (an infinite number of tangent circles exists).

#### GetVectorDistance2D
```cs
float GetVectorDistance2D(Vector3 vector)
```
GetVectorDistance2D returns the magnitude of a vector while ignoring the Z-component. Useful in 2D scenarios.

* ```vector (Vector3)``` The vector to use.

## Physicsga
#### CreateTrajectoryRenderer
```cs
TrajectoryRenderer CreateTrajectoryRenderer(Transform parentTransform)
```
CreateTrajectoryRenderer automatically creates a GameObject, attaches a TrajectoryRenderer component to it, and hides it in the hierarchy for convenience. If you want to manually add the component to an existing GameObject, use AddComponent<TrajectoryRenderer>() instead.

* ```parentTransform (Transform)``` The transform that should parent the trajectory renderer. In most cases, it should be the transform of the projectile itself.

## Renderers
#### TrajectoryRenderer

TrajectoryRenderer is a component that predicts trajectories of moving objects and renders them in world space. Its function, appearance, and behavior are customizable according to the following properties:

* ```Gravity (Vector2)``` Sets the gravity that applies to the object in question.

* ```TimeStep (float)``` Sets the timestep, in seconds, between each consecutive rendered point on the trajectory. A lower timestep will render more points on the trajectory, while a higher one will render fewer points. For example, a timestep of 1 makes the renderer draw a point every 1 second in time. Lowering the timestep to 0.1 makes it draw a point every 0.1 seconds, so more points will be drawn overall.

* ```LineOfSight (float)``` Sets how far along the future, in world units, will the trajectory be shown. A line of sight of 1 means the object's trajectory will be traced until it will have traversed only 1 world unit from its initial position. Increasing it to 5 will show the first 5 world units' length of its trajectory.

* ```LineOfSightConstraint (DistanceMeasure)``` Sets the metric that will be used to measure the LineOfSight. "Actual" uses the actual distance traversed. "Horizontal" uses the horizontal distance traversed. "Vertical" uses the vertical distance traversed. By default, the metric is set to Actual.

* ```LineOfSightFadeType (FadeType)``` Sets how the points themselves will fade out the further they lie on the trajectory. The closer the points are to the beginning of the trajectory, the more opaque they will be, and they get more transparent until the end of the trajectory is reached. Options are "None", "Linear", and "Sinusoidal". "None" makes all the points fully opaque. "Linear" makes the points fade out linearly. "Sinusoidal" is not yet implemented as of the latest commit. By default, it is set to "None". 

* ```Opacity (float)``` Sets the opacity of the rendered trajectory. 0 makes it fully transparent; 1 makes it fully opaque.

* ```Sprite (Sprite)``` Sets the sprite of the points on the trajectory.

* ```FadeInType (FadeType)``` Not to be confused with the LineOfSightFadeType, the FadeInType controls how the _entire_ trajectory fades in once it is shown using the Show() method. "None" makes the trajectory show at once, "Linear" makes it fade in linearly, and "Sinusoidal" is not yet supported as of the latest commit. By default, it is set to "Linear".

* ```FadeInTime (float)``` Sets the time, in seconds, it takes the renderer to fade in. By default, it is set to 0.25 seconds.

* ```FadeOutType (FadeType)``` Similar to FadeInType, but applies when the trajectory is hidden using the Hide() method.

* ```FadeOutTime (float)``` Similar to FadeInTime.

After the TrajectoryRenderer is configured, it has to be constantly notified of any initial velocity changes. Be sure to call its OnInitialVelocityUpdate(Vector2) method whenever applicable.


Here's an example configuration:

```
TrajectoryRenderer trajectoryRenderer = Physicsga.CreateTrajectoryRenderer(transform);
trajectoryRenderer.Gravity = new Vector2(0f, Constants.GRAVITY.y * GRAVITY_SCALE);
trajectoryRenderer.LineOfSight = 5f;
trajectoryRenderer.TimeStep = 0.033f;
trajectoryRenderer.LineOfSightFadeType = TrajectoryRenderer.FadeType.Linear;
trajectoryRenderer.FadeOutType = TrajectoryRenderer.FadeType.None;
trajectoryRenderer.Sprite = AppManager.Instance.SpriteDatabase.GetSprite(SpriteId.TrajectoryDot);
trajectoryRenderer.Show();
```

Somewhere in FixedUpdate:

```
trajectoryRenderer.OnInitialVelocityUpdate((slingShotAnchorPoint - transform.position) * LAUNCH_FORCE_COEFFICIENT);
```

# Examples
The following are some examples of where and how I use LibraMage in my projects.
#### Fat Bunny: Rounded Corners
![Fat Bunny](img/GetTangentCircleExample.png)

In Fat Bunny, the lines of the mountainous terrain have rounded corners at the places where they intersect. Those rounded corners are actually part of a circle that is tangent to the sloped line and the flat line. Mathga.GetCircleTangentToTwoLines is being used to find the circle. 


#### Fat Bunny: Trajectory Prediction
![Fat Bunny](img/TrajectoryPredictionExample.png)

In Fat Bunny, one feature we've decided to experiment with is trajectory rendering. It shows the path the bunny will follow in the air after he jumps. Physicsga.CreateTrajectoryRenderer is being used to get such a trajectory renderer.