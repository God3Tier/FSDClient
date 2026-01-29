How to run
- It doesnt...
So essentially, each entry point is the .tscn file. Currently, I am on a branch working on the main game loop .tscn board.
To "run the code", you must launch it from the Godot engine with .NET support. The plan was main .tscn has an instance of each state
that the user is in (i.e. if they are in a deck building phase, waiting for match phase etc.) which is their own seperate .tscn file

Then, if you attach a script from the .tscn, it expects the 2 following format

public partial class MyClass : SomeNodeType 
{

  public override void _Ready() 
  {
    Whatever you want to test when the node enters scene for first time
  }
  public override void _Process(double delta)
  {
    However you wish to render the board
  }
}

My advice is just force the _Ready() to print everything under the sun out in the terminal then thats it. Otherwise, best of luck 
doing the UI math 

Since I have made the interesting decision of doing this in C#, I recommend using whatever text editor u r using with an lsp (Zed one 
not bad) while explicitely writing your code and the Godot Engine when trying to attach UI to the thing

Good luck guys
