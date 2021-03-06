# XR depth subsystem

The depth subsystem is an interface into depth information detected in the scene. This refers to feature points, which are unique features detected in the environment that can be correlated between multiple frames. A set of feature points is called a point cloud.

The depth subsystem is a type of [tracking subsystem](index.html#tracking-subsystems). Its trackable is [`XRPointCloud`](../api/UnityEngine.XR.ARSubsystems.XRPointCloud.html).

Some providers only have one `XRPointCloud`, while others might have several. Check your provider's documentation for more details.
