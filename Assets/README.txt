

					[][][][][][][][][][][][][][][][][][][]
					[][][][][][][][][][][][][][][][][][][]
					[][]							  [][]
					[][]  CMN5201 PRODUCTION PROJECT  [][]
					[][]      PROGRAMMING README	  [][]
					[][]							  [][]
					[][][][][][][][][][][][][][][][][][][]
					[][][][][][][][][][][][][][][][][][][]


>> INTRODUCTION <<

Greetings! Morgan here!

This is a simple readme document to explain a few things about the
programming side of this project, specifically for any group members also
working on it! You could say I have a "very particular" way of doing things,
so this is where I'll bring you up to speed with what that actually is, and
how you can make use of it.


>> CORE.CS <<

One of the scripts in the main "scripts" folder of this project is something
named "Core.cs". This is a class that contains a number of methods, objects,
and properties that you might want to have accessible from any other
class/script in the project. To make use of this, you simply have to replace
"MonoBehaviour" at the top of the class declaration with "Core". Maybe open
the script and have a scroll through to get a sense for what it places at
your disposal!


>> ... \ ASSETS \ RESOURCES <<

You may have noticed that one of the folders within the base Assets folder
is named "Resources", and within that folder is where many of the folders
you might expect to find directly within the Assets folder are actually
located. The reason for this is that it's possible to load assets - whether
images, models, materials, or so on - through code, rather than having to
apply them in the inspector, but this requires the assets to be in the
Resources folder or one of its subfolders.

Now, using the inspector is much easier, and generally the better option.
However, sometimes you *do* want to be able to load an asset purely through
code, and so it's better to set up the file structure ahead of time to
make doing this possible.


>> CONTROLS <<

You are, of course, familiar with getting Unity to do something when a key
is pressed, as it's the core means by which we allow a player to interact
with and control the game. However, in order to allow the player to change
the game's control layout, we need to do something a bit more complex. This
is where the "Controls" class comes in. Within the "Core" class, there is
the following line of code:

	public static Controls Controls;

This creates an object that is accessible by any class that inherets from
Core. This changes exactly how you want to go about receving inputs, so I'll
explain that once I've covered the format of the script that handles the
game's controls.

If you look at the code of the Controls.cs script, you'll see that it
contains the Controls class itself, as well as a number of other classes.
Each of these other classes is a category, and these categories exist for
the sake of organisation, especially when we come to making a settings menu.
The main controls class contains one instance of each category class, all of
them given a shortened name.

In each category class, you'll find the actual controls themselves. These
are set up to be a special class too, as that way it's also possible to
assign a display name to them. You can then check for whether a key is
held/pressed/released in one of two ways. The first of these is via the
standard Unity method, using this format:

	Input.GetKey(Controls.<Category>.<Control>.Key);
	Input.GetKeyDown(Controls.<Category>.<Control>.Key);
	Input.GetKeyUp(Controls.<Category>.<Control>.Key);

However, for the second approach, I added a set of equivalent methods to the
Core class, so if the method you want to get an input for is in a class that
inherits from Core, then you can use this format instead:

	GetInput(Controls.<Category>.<Control>);
	GetInputDown(Controls.<Category>.<Control>);
	GetInputUp(Controls.<Category>.<Control>);

Both approaches will work just fine, so it's up to you which you use!

But what if the control you need doesn't exist? Well, to prevent this
section from getting too long, I'll be adding comments to the Controls.cs
script to explain how to do that!

