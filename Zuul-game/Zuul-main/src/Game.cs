using System;
using System.Reflection.Metadata;

class Game
{
	// Private fields
	private Parser parser;
	private Player player;
	private bool keyUsed = false;
	Item key = new Item(15, "Key");

	Item medkit = new Item(20, "Medkit");
	// Constructor
	public Game()
	{
		parser = new Parser();
		player = new Player();
		CreateRooms();
	}

	// Initialise the Rooms (and the Items)
	private void CreateRooms()
	{
		// Create the rooms
		Room outside = new Room("outside the main entrance of the university");
		Room theatre = new Room("in a lecture theatre");
		Room pub = new Room("in the campus pub");
		Room lab = new Room("in a computing lab");
		Room office = new Room("in the computing admin office");
		Room hallway = new Room("in the university hallway");
		Room up = new Room("on the second floor of the university");
		Room storage = new Room("in the storage room");
		// Initialise room exits
		outside.AddExit("east", theatre);
		outside.AddExit("south", lab);
		outside.AddExit("west", pub);

		theatre.AddExit("west", outside);
		theatre.AddExit("door", hallway);

		hallway.AddExit("theatre", theatre);
		hallway.AddExit("stairs", up);

		up.AddExit("down", hallway);
		up.AddExit("storageroom", storage);

		storage.AddExit("hallway", hallway);

		pub.AddExit("east", outside);

		lab.AddExit("north", outside);
		lab.AddExit("east", office);

		office.AddExit("west", lab);

		theatre.Chest.Put("key", key);
		theatre.Chest.Put("medkit", medkit);
		// Start game outside
		player.CurrentRoom = outside;
	}

	//  Main play routine. Loops until end of play.
	public void Play()
	{
		PrintWelcome();

		// Enter the main command loop. Here we repeatedly read commands and
		// execute them until the player wants to quit.
		bool finished = false;
		while (!finished)
		{
			Command command = parser.GetCommand();
			finished = ProcessCommand(command);
		}
		Console.WriteLine("Thank you for playing.");
		Console.WriteLine("Press [Enter] to continue.");
		Console.ReadLine();
	}

	// Print out the opening message for the player.
	private void PrintWelcome()
	{
		Console.WriteLine();
		Console.WriteLine("Welcome to Zuul!");
		Console.WriteLine("Zuul is a new, incredibly boring adventure game.");
		Console.WriteLine("Type 'help' if you need help.");
		Console.WriteLine();
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));
	}

	// Given a command, process (that is: execute) the command.
	// If this command ends the game, it returns true.
	// Otherwise false is returned.
private bool ProcessCommand(Command command)
{
	bool wantToQuit = false;

	if (!player.IsAlive() && command.CommandWord != "quit")
	{
		Console.WriteLine("You bled out, you died...");
		Console.WriteLine("You can only use the command:");
		Console.WriteLine("quit");
		return wantToQuit;
	}

	if (keyUsed && command.CommandWord != "quit") 
	{
		Console.WriteLine("You have won the game, the only allowed command is 'quit'.");
		return wantToQuit;
	}

	if (command.IsUnknown())
	{
		Console.WriteLine("I don't know what you mean...");
		return wantToQuit;
	}

    switch (command.CommandWord)
    {
        case "help":
            PrintHelp();
            break;
        case "look":
            Look();
            break;
		case "take":
			Take(command);
			break;
		case "drop":
			Drop(command);
			break;
        case "status":
            Health();
            break;
        case "go":
            GoRoom(command);
            break;
		case "use":
			UseItem(command, out keyUsed); 
			break;

		case "quit":
			wantToQuit = true;
			break;
	}

	return wantToQuit;
}

	// ######################################
	// implementations of user commands:
	// ######################################
	
	// Print out some help information.
	// Here we print the mission and a list of the command words.
	private void PrintHelp()
	{
		Console.WriteLine("You are lost. You are alone.");
		Console.WriteLine("You wander around at the university.");
		Console.WriteLine();
		// let the parser print the commands
		parser.PrintValidCommands();
	}

	private void Look()
	{
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));

		Dictionary<string, Item> roomItems = player.CurrentRoom.Chest.GetItems();
		if (roomItems.Count > 0)
		{
			Console.WriteLine("Items in this room:");
			foreach (var itemEntry in roomItems)
			{
				Console.WriteLine($"{itemEntry.Value.Description} - ({itemEntry.Value.Weight} kg)");
			}
		}
	}


	private void Take(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("Take what?");
			return;
		}

		string itemName = command.SecondWord.ToLower();

		bool success = player.TakeFromChest(itemName);

	}

	private void Drop(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("Drop what?");
			return;
		}

		string itemName = command.SecondWord.ToLower();

		bool success = player.DropToChest(itemName);


	}

	private void Health()
	{
		Console.WriteLine($"Your health is: {player.GetHealth()}");

		Dictionary<string, Item> items = player.GetItems();

		if (items.Count > 0)
		{
			Console.WriteLine("Your current items:");

			// Iterate over elk item in player zijn inv
			foreach (var itemEntry in items)
			{
				Console.WriteLine($"- {itemEntry.Key}: Weight {itemEntry.Value.Weight}");
			}
		}
		else
		{
			Console.WriteLine("You have no items in your inventory.");
		}
	}

	
	// Try to go to one direction. If there is an exit, enter the new
	// room, otherwise print an error message.
	private void GoRoom(Command command)
	{
		if(!command.HasSecondWord())
		{
			// if there is no second word, we don't know where to go...
			Console.WriteLine("Go where?");
			return;
		}

		string direction = command.SecondWord;

		// Try to go to the next room.
		Room nextRoom = player.CurrentRoom.GetExit(direction);
		if (nextRoom == null)
		{
			Console.WriteLine("There is no door to "+direction+"!");
			return;
		}

		player.Damage(25);
		player.CurrentRoom = nextRoom;
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));
		if (player.CurrentRoom.GetExit("door") != null)
		{
			Console.WriteLine("You found a door in the theatre, it seems like it leads to the university hallway.");
		}
		
		if (!player.IsAlive()) 
		{
			Console.WriteLine("Your vision blurs, the world fades. Your wounds draining your strength. You collapse, you have bled out..");
		}
	}

    private void UseItem(Command command, out bool keyUsed)
    {
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Use what?");
            keyUsed = false;
            return;
        }

        string itemName = command.SecondWord.ToLower();

        bool itemUsed = player.Use(itemName, out keyUsed);

        if (itemUsed)
        {
            if (keyUsed)
            {
                this.keyUsed = true; 
                Console.WriteLine("You called 911, an ambulance and police are on their way...");
				Console.WriteLine("Your vision blurs as you lose consciousness...");
				Console.WriteLine("You regain consciousness later, you are in an ambulance...");
				Console.WriteLine(" ");
				Console.WriteLine("Congratulations, you have won the game.");
            }
        }
    }
}




