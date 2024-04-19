class Player
{
    public Room CurrentRoom { get; set; }
    private int health;
    private Inventory backpack;

    // Constructor
    public Player()
    {
        CurrentRoom = null; 
        health = 100;
        backpack = new Inventory(25);
    }
    
    public Dictionary<string, Item> GetItems()
    {
        return backpack.GetItems();
    }

    public int GetHealth()
    {
        return health;
    }


    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0)
            health = 0;
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > 100)
            health = 100;
    }

    public bool IsAlive()
    {
        return health > 0;
    }

    public bool TakeFromChest(string itemName)
    {
        Item item = CurrentRoom.Chest.Get(itemName);

        if (item != null) // item gevonden in kamer
        {
            bool success = backpack.Put(itemName, item);
            
            if (success)
            {
                // Remove the item from the chest
                CurrentRoom.Chest.Get(itemName);
                Console.WriteLine($"You took the {itemName}.");
                return true;
            }
            else
            {
                CurrentRoom.Chest.Put(itemName, item);
                Console.WriteLine("You can't carry that. It's too heavy.");
                return false;
            }
        }
        else
        {
            Console.WriteLine($"There is no {itemName} here.");
            return false;
        }
    }

    public bool DropToChest(string itemName)
    {
        Item item = backpack.Get(itemName);

        if (item != null) // Item found in inventory
        {
            bool success = CurrentRoom.Chest.Put(itemName, item);

            if (success)
            {
                backpack.RemoveWeight(item.Weight);

                Console.WriteLine($"You dropped the {itemName}.");
                return true;
            }
            else
            {
                backpack.Put(itemName, item);
                return false;
            }
        }
        else
        {
            Console.WriteLine($"You don't have {itemName} in your backpack.");
            return false;
        }
    }


    public bool Use(string itemName, out bool keyUsed)
    {
        keyUsed = false; 
        if (backpack.GetItems().ContainsKey(itemName))
        {
            if (itemName.ToLower() == "medkit")
            {
                health += 25;
                if (health > 100)
                {
                    health = 100;
                }

                backpack.GetItems().Remove(itemName);
                backpack.RemoveWeight(20);
                Console.WriteLine($"You used the medkit. Your health is now {health}.");
                return true; 
            }
            else if (itemName.ToLower() == "key")
            {
                keyUsed = true; // Set keyUsed to true
                backpack.GetItems().Remove(itemName);
                Console.WriteLine("You used the key.");
                backpack.RemoveWeight(15);
                return true; 
            }
            else
            {
                Console.WriteLine("You can't use that item.");
                return false; 
            }
        }
        else
        {
            Console.WriteLine("You don't have that item in your inventory.");
            return false; 
        }
    }

}

