using System.Collections.Generic;

namespace GildedRoseKata;

public class GildedRose
{
    const string Sulfuras = "Sulfuras, Hand of Ragnaros";
    const string AgedBrie = "Aged Brie";
    const string BackstagePasses = "Backstage passes to a TAFKAL80ETC concert";

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
        // Legendary: never has to be sold and never degrades. Returning early is
        // behavior-preserving because every branch below already excluded it by
        // name, so those inner guards can go with it.
        if (item.Name == Sulfuras)
        {
            return;
        }

        if (item.Name == AgedBrie)
        {
            UpdateAgedBrie(item);
            return;
        }

        if (item.Name == BackstagePasses)
        {
            UpdateBackstagePasses(item);
            return;
        }

        if (item.Name != "Aged Brie" && item.Name != "Backstage passes to a TAFKAL80ETC concert")
        {
            if (item.Quality > 0)
            {
                item.Quality = item.Quality - 1;
            }
        }
        else
        {
            if (item.Quality < 50)
            {
                item.Quality = item.Quality + 1;

                if (item.Name == "Backstage passes to a TAFKAL80ETC concert")
                {
                    if (item.SellIn < 11)
                    {
                        if (item.Quality < 50)
                        {
                            item.Quality = item.Quality + 1;
                        }
                    }

                    if (item.SellIn < 6)
                    {
                        if (item.Quality < 50)
                        {
                            item.Quality = item.Quality + 1;
                        }
                    }
                }
            }
        }

        item.SellIn = item.SellIn - 1;

        if (item.SellIn < 0)
        {
            if (item.Name != "Aged Brie")
            {
                if (item.Name != "Backstage passes to a TAFKAL80ETC concert")
                {
                    if (item.Quality > 0)
                    {
                        item.Quality = item.Quality - 1;
                    }
                }
                else
                {
                    item.Quality = item.Quality - item.Quality;
                }
            }
            else
            {
                if (item.Quality < 50)
                {
                    item.Quality = item.Quality + 1;
                }
            }
        }
    }

    // Gains 1 a day, 2 within 10 days of the concert and 3 within 5, never above
    // 50 — then becomes worthless the day after.
    static void UpdateBackstagePasses(Item item)
    {
        if (item.Quality < 50)
        {
            item.Quality = item.Quality + 1;

            if (item.SellIn < 11 && item.Quality < 50)
            {
                item.Quality = item.Quality + 1;
            }

            if (item.SellIn < 6 && item.Quality < 50)
            {
                item.Quality = item.Quality + 1;
            }
        }

        item.SellIn = item.SellIn - 1;

        if (item.SellIn < 0)
        {
            item.Quality = 0;
        }
    }

    // Gains 1 a day, and 1 again once past the sell-by date, never above 50.
    static void UpdateAgedBrie(Item item)
    {
        if (item.Quality < 50)
        {
            item.Quality = item.Quality + 1;
        }

        item.SellIn = item.SellIn - 1;

        if (item.SellIn < 0 && item.Quality < 50)
        {
            item.Quality = item.Quality + 1;
        }
    }
}
