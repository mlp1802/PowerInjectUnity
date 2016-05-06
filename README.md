## Primal_Inject
###Introduction
Welcome to the primal inject documentation page. Primal\_Inject is a dependency injection framework specifically designed for Unity. It is designed to fit right into any Unity project with a minimum of modifications to your existing code. Everything is setup using annotations so you will not need to inherit from specific classes or similar intrusive behavior. Any existing project can be gradually ported to Primal\_Inject gradually and as needed.

Why would you need Primal\_Inject, or any other Dependency Framework, in Unity3D anyway ? in short, to reference other objects that are not part of the same GameObject.
When I say objects I mean both your MonoBehaviors as well as any other custom object.
Unity3Ds "component model" is great for most cases, but what happens when you need to access an object not belonging to the same GameObject (as the one you are currently "in" )? This is where Primal_Inject steps in.


###Injection
A quick example of how to "produce" things and how to "inject" things

	using PrimalInject;

    [Insert]
    class Player:MonoBehaviour {
        void Update() {...}   
    }

 This set up a "player" object. Any mono behavior annotated with [Insert] is marked as an object that can be injected into other objects. It will also itself be injected.

The Player object can be now used from any other object directly under the "pipeline" (I will get back to what a "pipeline" is in a minute)..like this.

	using PrimalInject;
     [Insert]
    class SomeLevel:MonoBehaviour {
        [Inject]
		public Player player;//receives player here
    }
	
The `SomeLevel`  behavior receives the `Player` object and also makes itself available for other objects like this:

    [Primal]
    class DoSomeStuffWithLevel:MonoBehaviour {
        [Inject]
        SomeLevel level;
        void Update() {
            MonoBehaviour.print("We got player "+level.player+" here !");
        }
    }
Note that in this case, the "DoSomeStuffWithLevel" class is not marked with "Insert", but simply with the annotation`Primal`. You should do that in cases where you do not want to make the object available for other objects, but simply want it to be injected.

> You must always mark your MonoBehaviors with either [Insert] or [Primal] or Primal\_Inject will not notice them. The reason is that you might have thousands of other monobehaviors that you might not want to inject, and to save resources Primal_Inject only considers monobehaviors marked with either [Insert] or [Primal] as objects that needs injection.


This is basic injection, we will get back to more advanced forms of injections, *named and typed*, later.


###When will objects be available ?

Dependency injection will be executed AFTER the monobehaviors `Start()` method. So do not try to use any objects (that require to be injected) in the `Awake()` or  `Start()`methods.

All objects will be available and fully injected on FixedUpdate and Update.

But if you need a method similar to `Start(), but`where all objects are ready for use, you can annotate a method with the [OnInjected] attribute like this:

    class Player {
			
			[Inject]
	        IUserControls controls;                
            
            [OnInjected]
            void init() 
            {
				//The "controls" variable is now available.
            }
        }

This method will be executed AFTER `Start()` but BEFORE  `Update()` and `FixedUpdate()`, so you can do any last minute initialization here.
###Producing objects
You can *produce* objects using either the `[Insert]` tag or the `[Produce]` tag.  Insert tag works, as mentioned, like this.

	[Insert]
    class SomeMuchNeededBehavior:MonoBehaviour
    {
    }

But you can "produce" objects in another way as well

Let's say we have these two classes:


    class BulletSpawner {
        //..
    }
    class UserControls {
        //..
    }

	
We want to create them and make them available for injection. 

	[Primal]
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

Here we have a monobehavior which sole responsibility is to create other objects, using the "produce" tag.

But what if some of our objects are dependent on other objects in the creation phase ? Lets say, we need a "Settings" object to be available before we can create a "UserControls" object. No worry, simply add the `UserSettings` to the "producer signature", like this:
	
	

	    [Produce]
        UserControls createUserControls(Settings settings) {
            var speed = settings.speed;
            return new UserControls(speed);
        }

> Note: some of my examples are pretty silly because in very simple applikations, you often do not need
> dependency injection. I hope that they make sense though.

You can receive as many objects as you want:
	
	

    [Produce]
        UserControls createUserControls(Settings settings,SomeStuff stuff) {
            return new UserControls(settings,stuff);
        }


   Of course, this producer will not be called until *other* producers methods somewhere produced a *Settings* objects and a *SomeStuff* object.
   
The [Insert] tag is a convenience tag. Instead of writing this:

    [Primal]
    class Player:MonoBehaviour {
        [Produce] Player letsGetThePlayer() {
            return this;
        }
    }
    
It's quicker to write this:
	

    [Insert]
    class Player:MonoBehaviour {
	        
    }

   It works in the exact same way.

###Interface composition

In many cases you would like to implement some functionality but you are not quite sure of the scope of it. 
You might have an idea of it, or you at least partly know what it should do.
The goal is to create a single interface implementation that contains the desired functionality, but the problem is that that the functionality, and the necessary data needed to create it, might be spread out all over the application.

Let's say we have a `IUserControl` interface, that returns output like "move forward, go back, fire" etc. The basic implementation is pretty straightforward, but what if you need to do special checks for other things that you cannot even imagine at this time of development ? You might want to, at a later stage, check if the player are low on energy, and perhaps can't use the command "fire" anymore.. Or on level 21, you can't fire your weapon. But you do not want to cramp all that information into one single class because it will be huge and difficult to maintain.

Ok we have the interface:
	

    interface IUserControls {
        public string getCommand();
    }
   
  And an implementation that reads the keyboard:
	

    public class KeyboardUserControls:IUserControls {
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

At first you make the `KeyboardUserControls`  available:

    [Primal]
    class SomeBasicSetupBevavior : MonoBehaviour
    {
        [Produce]
        IUserControls createControls() {
            return new KeyboardUserControls();
        }
    }


At some point in the future, you realize that you want to expand this interface to contain more stuff, but you want to keep the `KeyboardUserControls` as simple as possible, honoring the Single Responsibility principle as much as possible, and ensuring that, when your project has grown to two million lines of code, you can maintain your sanity.
Let's say, we have a `HealtCheckerControls` class that implements the `IUserControls` interface. This class checks for players energy  and prevents the player from moving when energy is less that zero:
	

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
                //when we have energy, just use previous interface.
                return controls.getCommand();
            }
        }
    }

 Then you need to add a new producer method that receives the *old implementation and returns a new one*.

    [Primal]
    class EvenMoreSetupAddedLater {
        [Produce]
        IUserControls enhanceControls(IUserControls controls)//receives IUsercontrols, returns IUserControls
        {
            return new HealtCheckerControls(controls);
        }
    }
    
	
Of course you can receive more objects as well:

	       [Produce]
            IUserControls enhanceControls(SomeObject someObj,IUserControls controls)//receives IUsercontrols, returns IUserControls
            {
                
	            return new HealtCheckerControls(controls,someObj);
            }
         
> **Note that the name of the producer methods are not important at all. They just have to be annotated with the *[produce]* annotation.**

###Order of execution

Producers are executed whenever the objects they need becomes available (the arguments) and in this order:

1) Top down in the scenegraph.

2) The monobehaviors position on the GameObject (top down)

3) The producer functions position in the MonoBehavior (top down)

If order of execution is important (in my experience, it rarely is), simply rearrange the producers position.

Producers that returns the same type as it receives will always execute before any producer that receives another type that it returns .

Example

	    //this method "enhances" IUserControls
	    public IUserControls produce1(IUserControls controls) { 
            ..
        }
		//this method creates a player and needs a IUserControls to do it. Thus it //receives the FINAL implementation of IUsercontrols
        public Player produce2(IUserControls controls) { 
            ..
        }

In this case, `Produce1` will execute before `Produce2. `This is because `Produce1`is a part of an interface composition (it adds to an interface) whereas `Produce2` probably needs the "final" implementation of  `IUserControls.` 

###Named injection

As it is, there can only be ONE instance of each class or each interface in the object graph at one time. If you have produced a "player", that is the only player available. No matter how many players you produce, only the last one will be available. Likewise, there will only be only one instance of each type of interface.

To get around of that, you can "name" the objects produced. 

	

		    [Produce(Named = "Player1")]
            public Player createPlayer1() {
                ...
            }
    
            [Produce(Named = "Player2")]
            public Player createPlayer2()
            {
                ...
            }

And you can pick them up either in other producers or through injection, like this:

In producers:

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

Through injection:

    class ClassThatNeedsPlayers {
        [Inject]
        [Named("Player1")]
        Player player1;
        
        [Inject]
        [Named("Player2")]
        Player player2;
    }

###Type producing

The return type of each producer is also the exact type it binds to.
The object created by:
		

   	    [Produce]    
           public IUserControls createControls() {
               return new KeyboardUserControls();
           }


Will be received only by:
		

		 [Inject]
         IUserControls controls;

         //and.. 
        
        [Produce]
        Player producePlayer(IUserControls controls)
        {
            ..
        }
etc..


Not by 
	
		[Inject]
        KeyboardUserControls controls;
		//or..
		[Produce]
        Player producePlayer(KeyboardUserControls controls)
        {
            ..
        }

 Because the producer method `createControls()` returns an `IUserControls`, and not a `KeyboardUserControls` 
       
The exact type must match. It is not enough that the two types are compatible. You can, however, specify that your producer or field (to be injected)  would  like to receive only a specific implementation.
You will use the `[Typed]` annotation for this:

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

You can also specify `[Typed]` when using the `[Insert]` tag on monobehaviors. 


    [Insert (Typed=typeof(IUserControl))]
    class MonoBehaviorThatIsAlsoAUserControl:IUserControls {
	...
	}

Here you specify that your monobehavior should really be seen as a `IUserControls`and not a `MonoBehaviorThatIsAlsoAUserControl`.
If you do not specify that, it will be seen as a `MonoBehaviorThatIsAlsoAUserControl`and no field or producer that expects a `IUserControls` will ever receive it..


###The [NewInstance] attribute.
If you want to create and object only used in a single class, meaning it is not injected into other classes, you can use the [NewInstance] tag. It is very similar to the `new` keyword, only difference is that objects created with `new` will not be injected.

    class Player {
		 Gun gun1 = new Gun();//gun1 will not be injected
         [NewInstance]
         Gun gun2;//gun2 will be injected fully
            
       }
       
	class Monster {
         [NewInstance]
         Gun gun;
            
       }
       
       
Both `Player`	 and `Monster` will have a fully injected `Gun`s, but they will be separate instances.

When using the  `[NewInstance]` attribute,  the usual inject attributes works in the newly created object but you can use constructor injection as well. The constructor need to be annotated with the `[Inject]` attribute.
The framework will selected the constructor with the most parameters first.

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

###Creating objects on the fly
But what if you want to dynamically create objects ? Bullets, for example. Attributes can't handle that.
The solution is to inject the injector itself, then inject the newly created object, using the injector. That sentence is barely understandable so here is an example.

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
       
Just remember that injection might be an expensive operation if you have a deep object graph, so some kind of object pooling might be appropriate.




 
	 




