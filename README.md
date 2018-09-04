
### PowerInject

Power_Inject is a dependency injection framework specifically designed for Unity. It is designed to fit right into any Unity project because it does not require you to modify your existing code in any way.

Everything is configured using attributes so you will not need to inherit from specific classes or similar intrusive behavior. Any existing project can be ported to Power_Inject gradually and as needed.
Why would you need Power_Inject, or any other dependency injection framework, in Unity3D anyway ? in short, to allow objects that are normally too “far away from each to” to reference each other.

When I say objects I mean both your MonoBehaviors as well as any other custom object.
Unity3Ds “component model” is great for most cases, but what happens when you need to access an
object not belonging to the same GameObject (as the one you are currently “in” )? This is where Power_Inject steps in.

### Setup

Import the package into your project. Select a gameobject and add a PowerPipeline component to it. Any `MonoBehavior` that uses injection must be placed at this GameObject or anywhere below it in the scene graph hierarchy.

A quick example of how to “produce” things and how to “inject” things

```csharp


using PowerInject;

[Insert]
class Player:MonoBehaviour {
    void Update() {...}   
}
```

This sets up a “Player” object. Any ```MonoBehavior annotated with [Insert] is marked as an object that can be injected into other objects. It will also itself be injected.
The Player object can now be accessed from any other object below the PowerPipeline component by using the [Inject] attribute

```csharp
using PowerInject;
[Insert]
class SomeLevel:MonoBehaviour {
    [Inject]
    Player player;//receives player here
}
```


You can use properties as well:

```csharp
using PowerInject;
 [Insert]
class SomeLevel:MonoBehaviour {
    [Inject]
    public Player player {get;set;};//receives player here
}
```

The `SomeLevel` behavior receives the Player object and also makes itself available for other objects:
```csharp
[Power]
class DoSomeStuffWithLevel:MonoBehaviour {
    [Inject]
    SomeLevel level;
    void Update() {
        MonoBehaviour.print("We got player "+level.player+" here !");
    }
}
```


Note that in this case, the `DoSomeStuffWithLevel` class is not marked with `[Insert]`, but simply with the annotation `[Power]`. You should do that in cases where you do not want to make the object available for other objects, but simply want it to be injected.

You must always mark your MonoBehaviors with either `[Insert]` or `[Power]` or the framework will not notice them. The reason is that you might have thousands of other MonBehaviors that you might not want to inject, and to save resources Power_Inject only considers monobehaviors marked with either [Insert] or [Power] as objects that need injection.

This is basic injection, we will get back to more advanced forms of injections, named and typed, later.

### When will objects be available ?

Dependency injection will be executed AFTER the monobehaviors Start() method. So you should not try to use any objects (that requires injection) in the Awake() or Start()methods.
All objects will be available and fully injected on FixedUpdate and Update.
But if you need a method similar to Start(), but where all objects are ready for use, you can attribute a method with the [OnInjected] attribute like this:

```csharp
class Player {

        [Inject]
        IUserControls controls;                

        [OnInjected]
        void init() 
        {
            //The "controls" variable is now available.
        } 
    }
```

This method will be executed AFTER Start() but BEFORE Update() and FixedUpdate(), so you can do any last minute initialization here.
The method attributed with [OnInjected] must have zero parameters

### Producing objects
You can produce objects using either the [Insert] attribute or the [Produce] attribute. The [Insert] attribute works, as mentioned, like this.

```csharp
[Insert]
class SomeMuchNeededBehavior:MonoBehaviour
{
}
````
But you can “produce” objects in another way as well
Let’s say we have these two classes:

```csharp

class BulletSpawner {
    //..
}
class UserControls {
    //..
}
````

We want to create them and make them available for injection.
```csharp
[Power]
class HereWeCreateAlotOfObjects : MonoBehaviour
{
    [Produce]
    BulletSpawner createBulletSpawner() {
        return new BulletSpawner();
    }

    [Produce]
    UserControls createUserControls() {
        return new UserControls();
    }
}
```

Here we have a monobehavior which sole responsibility is to create other objects, using the “produce” tag.
But what if some of our objects are dependent on other objects in the creation phase ? Lets say, we need a “Settings” object to be available before we can create a “UserControls” object. No worry, simply add the UserSettings to the “producer signature”, like this:
```csharp    
[Produce]
UserControls createUserControls(Settings settings) {
    var speed = settings.speed;
    return new UserControls(speed);
}

```
You can receive as many objects as you want:
```csharp
    [Produce]
    UserControls createUserControls(Settings settings,SomeStuff stuff) {
        return new UserControls(settings,stuff);
    }
```

Of course, this producer will not be called until other producers methods somewhere produced a Settings objects and a SomeStuff object.
The [Insert] attribute is a convenience attribute. Instead of writing this:
```csharp
[Power]
class Player:MonoBehaviour {
    [Produce] Player letsGetThePlayer() {
        return this;
    }
}
```

It’s quicker to write this:
```csharp
[Insert]
class Player:MonoBehaviour {

}
``` 

It works in the exact same way.

### Interface composition
In many cases you would like to implement some functionality but you are not quite sure of the scope of it.
You might have an idea of it, or you at least partly know what it should do.
The goal is to create a single interface implementation that contains the desired functionality, but the problem is that that the functionality, and the necessary data needed to create it, might be scattered out all over the application.
Let’s say we have a IUserControl interface, which has a function that returns output like “move forward, go back, fire” etc. The basic implementation is pretty straightforward, but what if you need to do special checks for other things that you cannot even imagine at this time of development ? You might want to, at a later stage, check if the player are low on energy, and perhaps can’t use the command “fire” anymore.. or on level 21, you can’t fire your weapon. But you do not want to cramp all that functionality into one single class because it will be huge and difficult to maintain.
Let’s define the interface:
```csharp
interface IUserControls {
    public string getCommand();
}
```


And an implementation that reads the keyboard:
```csharp
class KeyboardUserControls:IUserControls {
    public String getCommand()
    {           
        if (Input.GetKey(KeyCode.UpArrow))
        {
            return "move_forward";
        }
        //etc..
        return "";//nothing was selected    
    }
}
```

At first you make the KeyboardUserControls available:
```csharp
[Power]
class SomeBasicSetupBevavior : MonoBehaviour
{
    [Produce]
    IUserControls createControls() {
        return new KeyboardUserControls();
    }
}
```


At some point in the future, you realize that you want to expand this interface to contain more stuff, but you also want to keep the KeyboardUserControls simple, honoring the Single Responsibility principle as much as possible, and ensuring that, when your project has grown to two million lines of code, you will maintain your sanity.

Let’s say, we have a HealtCheckerControls class that implements the IUserControls interface. This class checks for players energy and prevents the player from moving when energy is less that zero:
```csharp
class HealtCheckerControls:IUserControls {
    [Inject]
    Player player;
    IUserControls controls;
    public HealtCheckerControl(IUserControls controls) {
        this.controls = controls;
    }
    public string getCommand() 
    {
        //check for energy too low
        if (player.energy < 0)
        {
            return "";//can't move when no energy.
        }
        else { 
            //when we have energy, just use the previous interface.
            return controls.getCommand();
        }
    }
}
````

Then you need to add a new producer method that receives the old implementation and returns a new one.
```csharp
[Power]
class EvenMoreSetupAddedLater {
    [Produce]
    IUserControls enhanceControls(IUserControls controls)//receives IUsercontrols, returns IUserControls
    {
        return new HealtCheckerControls(controls);
    }
}
```

Of course you can receive more objects as well:
```csharp
[Produce]
IUserControls enhanceControls(SomeObject someObj,IUserControls controls)//receives IUsercontrols, returns IUserControls
{
    return new HealtCheckerControls(controls,someObj);
}
```


The names of the producer methods are not important at all. They just have to be annotated with the [produce] annotation.

### Order of execution
Producers are executed whenever the objects they need becomes available (the arguments) and in this order:

1) Top down in the scene graph.
2) The monobehaviors position on the GameObject (top down)
3) The producer functions position in the MonoBehavior (top down)

If order of execution is important, simply rearrange the producers position.
Producers that returns the same type as it receives will always execute before any producer that receives another type that it returns .
Example
```csharp
//this method "enhances" IUserControls
public IUserControls produce1(IUserControls controls) { 
    ..
}
//this method creates a player and needs a IUserControls to do it. Thus it receives the FINAL implementation of IUsercontrols
public Player produce2(IUserControls controls) { 
    ..
}
```
In this case, Produce1 will execute before Produce2.This is because Produce1is part of an interface composition (it adds to an interface) whereas Produce2 probably needs the “final” implementation of IUserControls.

### Named injection
As it is, there can only be ONE instance of each class or each interface among the final produced objects at one time. If you have produced a “Player”, that is the only player available. No matter how many players you produce, only the last one will be available. Likewise, there will only be only one instance of each type of interface.
To get around of that, you can “name” the objects produced.
```csharp
[Produce(Named = "Player1")]
public Player createPlayer1() {
    ...
}

[Produce(Named = "Player2")]
public Player createPlayer2()
{
    ...
}
```


And you can pick them up either in other producers or through injection, like this:
In producers:
```csharp
[Produce]
IUserControls producePlayerControlsForPlayer1([Named("Player1")] Player   player)
        {
            ..
        }
 [Produce]
  IUserControls producePlayerControlsForPlayer2([Named("Player2")] Player player)
  {
       ..
  }
```

Injection:
```csharp
class ClassThatNeedsPlayers {
    [Inject]
    [Named("Player1")]
    Player player1;

    [Inject]
    [Named("Player2")]
    Player player2;
}
```
### Type producing
The return type of each producer is also the exact type it binds to.
The object created by:
```csharp
[Produce]    
   public IUserControls createControls() {
       return new KeyboardUserControls();
   }
```
Will be received only by:
```csharp
 [Inject]
 IUserControls controls;

 //and.. 

[Produce]
Player producePlayer(IUserControls controls)
{
    ..
}
```


Not by :
```csharp
[Inject]
KeyboardUserControls controls;
//or..
[Produce]
Player producePlayer(KeyboardUserControls controls)
{
    ..
}
```


Because the producer method createControls() returns an IUserControls, and not a KeyboardUserControls
The exact type must match. It is not enough that the two types are compatible. You can, however, specify that your producer or field (to be injected) would like to receive only a specific implementation.
You will use the [Typed] annotation for this:
```csharp
//injection..
[Inject]
[Typed(typeof(KeyboardUserControls))]
IUserControls controls;

//..and producer
[Produce]
Player producePlayer([Typed(typeof(KeyboardUserControls))] IUserControls controls)
{
    ...
}       
```

You can also specify [Typed] when using the [Insert] tag on monobehaviors.
```csharp
[Insert (Typed=typeof(IUserControl))]
class MonoBehaviorThatIsAlsoAUserControl:IUserControls {
...
}
```


Here you specify that your monobehavior should really be seen as a IUserControlsand not a MonoBehaviorThatIsAlsoAUserControl.
If you do not specify that, it will be seen as a MonoBehaviorThatIsAlsoAUserControland no field or producer that expects a IUserControls will ever receive it..

### The [NewInstance] attribute.
If you want to create an object only used in a single class, meaning it is not injected into other classes, you can use the [NewInstance] tag. It is very similar to the new keyword, only difference is that objects created with new will not be injected.
```csharp
class Player {
     Gun gun1 = new Gun();//gun1 will not be injected
     [NewInstance]
     Gun gun2;//gun2 will be injected fully
   }

class Monster {
     [NewInstance]
     Gun gun;   
   }
```

Both Player and Monster will have a fully injected Guns, but they will be separate instances.
When using the [NewInstance] attribute, the usual inject attributes works in the newly created object but you can use constructor injection as well. The constructor need to be annotated with the [Inject] attribute.
The framework will user the constructor with the most parameters.

```csharp
class Gun {
    [Inject]
    SomethingElse somethingElse;
    [Inject]
    public Gun([Named("Player1")]  Player player) { 
        ...        
    }
   [Inject]
    public Gun(Player player,Settings settings) { 
        ...        
    }
}
```

### Creating objects on the fly
But what if you want to dynamically create objects ? Bullets, for example. Attributes can’t handle that.
The solution is to inject the injector itself, then inject the newly created object, using the injector. That sentence is barely understandable so here is an example.
```csharp
class Player {
    [Inject]
    Injector injector;//the Injector interface is provided by the framework,
                      //is is not something that you need to produce.
    public void updatePlayer()
    {
        Bullet bullet = new Bullet();
        injector.inject(bullet);//now bullet is ready for use
        //..do stuff with bullet
    }
}
```

Just remember that injection might be an expensive operation if you have a deep object graph, so some kind of object pooling might be appropriate.

### Shielded injection
If you need areas of separate injection, meaning, objects graphs that do not interfere with each other, simply add more PowerPipelines to the scene graph.

### Hierarchical injection
Sometimes you want a “main” or “parent” pipeline, that floats objects down to sub pipelines. You can have as many pipelines as you want, ordered in whatever hierarchy that reflects the logic of your application. The pipelines on the same level do not know about each other, but all receive common objects from the mainpipeline.
An example could be a two player game with two identical “Player” pipelines, and a shared “Master” pipeline.
The master pipeline could provide objects like levels, settings etc, and the “player” pipelines objects like controls, health etc.
The set up could be something like this (in the scenegraph)
  
MasterPipeline  
--Player1 pipeline  
--Player2 pipeline  

### Error messages
When you run your game, you will receive warning messages in the Unity log window about what producers did not run and what fields were not injected, if any.
