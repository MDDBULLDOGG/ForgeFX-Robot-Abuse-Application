# ForgeFX-Robot-Abuse-Application

This mini-project takes a model with children and allows the user to 'attach' and 'detach' it's children to a defined 'root'. Additionally, the UI tracks the attachment status of all limbs.

In this example, you can pick apart all limbs and 'subcomponents' of an Iron Giant model.
Additionally, this system "should" work for any provided model with parent-child relationships provided you set up a prefab, however this functionality is untested.

In theory, to set up a new model:
1. Create a new prefab and add your model.
2. Add an 'Object Controller' script to the prefab, and assign your root (which will be immovable) and a Snap Indicator prefab.
3. Change the Core Controller's 'Prefab Object' to your new prefab.
4. Enter play mode!
