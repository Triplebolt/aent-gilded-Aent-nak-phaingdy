using System.Collections.Generic;

namespace GildedRoseKata;

public class GildedRose
{
    const string AgedBrie = "Aged Brie";
    const string Sulfuras = "Sulfuras, Hand of Ragnaros";
    const string BackstagePasses = "Backstage passes to a TAFKAL80ETC concert";
    const string ConjuredPrefix = "Conjured";

    const int MinQuality = 0;
    const int MaxQuality = 50;

    IList<Item> Items;

    public GildedRose(IList<Item> Items)
    {
        this.Items = Items;
    }

    public void UpdateQuality()
    {
        foreach (var item in Items)
        {
            UpdateItem(item);
        }
    }

    static void UpdateItem(Item item)
    {
        // Legendary: never has to be sold and never degrades.
        if (item.Name == Sulfuras)
        {
            return;
        }

        var daysRemaining = item.SellIn;
        item.SellIn = daysRemaining - 1;
        var expired = item.SellIn < 0;

        switch (item.Name)
        {
            case AgedBrie:
                Increase(item, expired ? 2 : 1);
                break;

            // Worthless once the concert has passed.
            case BackstagePasses when expired:
                item.Quality = MinQuality;
                break;

            case BackstagePasses:
                Increase(item, daysRemaining < 6 ? 3 : daysRemaining < 11 ? 2 : 1);
                break;

            // Conjured items degrade twice as fast as normal ones. Matched on
            // prefix rather than exact name because the spec describes a whole
            // supplier category, not the single "Conjured Mana Cake" we stock
            // today.
            case var name when name.StartsWith(ConjuredPrefix):
                Decrease(item, expired ? 4 : 2);
                break;

            default:
                Decrease(item, expired ? 2 : 1);
                break;
        }
    }

    // Stepwise rather than clamped so that items already outside [0, 50] keep
    // the behavior the original implementation gave them.
    static void Increase(Item item, int steps)
    {
        for (var i = 0; i < steps && item.Quality < MaxQuality; i++)
        {
            item.Quality++;
        }
    }

    static void Decrease(Item item, int steps)
    {
        for (var i = 0; i < steps && item.Quality > MinQuality; i++)
        {
            item.Quality--;
        }
    }
}
