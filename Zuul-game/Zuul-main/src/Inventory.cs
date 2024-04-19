using System.Collections.Generic;

class Inventory 
{
    private int maxWeight;
    private int currentWeight;
    private Dictionary<string, Item> items;

    // Constructor
    public Inventory(int maxWeight)
    {
        this.maxWeight = maxWeight;
        this.currentWeight = 0;
        this.items = new Dictionary<string, Item>();
    }


    public void RemoveWeight(int weight)
    {
        currentWeight -= weight;
    }

    public bool Put(string itemName, Item item)
    {
        // Check inventory space
        if (currentWeight + item.Weight <= maxWeight && !items.ContainsKey(itemName))
        {
            // Zet items in dictionary
            items[itemName] = item;
            currentWeight += item.Weight;
            return true;
        }
        return false;
    }

    public Item Get(string itemName)
    {
        // Zoek item in dictionary
        if (items.ContainsKey(itemName))
        {
            Item item = items[itemName];
            items.Remove(itemName);
            currentWeight -= item.Weight;
            return item;
        }
        return null;
    }

    public Dictionary<string, Item> GetItems()
    {
        return items;
    }
}
