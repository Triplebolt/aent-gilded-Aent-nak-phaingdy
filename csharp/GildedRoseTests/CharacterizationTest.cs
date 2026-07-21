using System.Collections.Generic;

using GildedRoseKata;

using Xunit;

namespace GildedRoseTests;

/// <summary>
/// Characterization tests. These pin the behavior the code has *today*, which is
/// not always the behavior GildedRoseRequirements.md asks for. Every expected
/// value here was derived by running the existing implementation, not by reading
/// the spec — where the two disagree, the code wins (see the Conjured tests).
///
/// Their job is to make a refactor of UpdateQuality safe: the 30-day approval
/// test tells you *that* something changed, these tell you *which rule* broke.
/// </summary>
public class CharacterizationTest
{
    const string Normal = "+5 Dexterity Vest";
    const string Brie = "Aged Brie";
    const string Sulfuras = "Sulfuras, Hand of Ragnaros";
    const string Backstage = "Backstage passes to a TAFKAL80ETC concert";
    const string Conjured = "Conjured Mana Cake";

    static Item UpdateOnce(string name, int sellIn, int quality)
    {
        var item = new Item { Name = name, SellIn = sellIn, Quality = quality };
        new GildedRose(new List<Item> { item }).UpdateQuality();
        return item;
    }

    static void AssertItem(Item item, int sellIn, int quality)
    {
        Assert.Equal(sellIn, item.SellIn);
        Assert.Equal(quality, item.Quality);
    }

    // ---- Normal items ----

    [Fact]
    public void NormalItem_BeforeSellBy_LosesOneQuality()
    {
        AssertItem(UpdateOnce(Normal, 10, 20), sellIn: 9, quality: 19);
    }

    [Fact]
    public void NormalItem_PastSellBy_LosesTwoQuality()
    {
        AssertItem(UpdateOnce(Normal, 0, 10), sellIn: -1, quality: 8);
    }

    [Theory]
    [InlineData(5, 0, 4, 0)]    // before sell-by, already at floor
    [InlineData(0, 0, -1, 0)]   // past sell-by, already at floor
    [InlineData(0, 1, -1, 0)]   // past sell-by, would double-decrement below 0
    public void NormalItem_QualityNeverGoesNegative(int sellIn, int quality, int expectedSellIn, int expectedQuality)
    {
        AssertItem(UpdateOnce(Normal, sellIn, quality), expectedSellIn, expectedQuality);
    }

    // ---- Aged Brie ----

    [Fact]
    public void AgedBrie_BeforeSellBy_GainsOneQuality()
    {
        AssertItem(UpdateOnce(Brie, 2, 0), sellIn: 1, quality: 1);
    }

    [Fact]
    public void AgedBrie_PastSellBy_GainsTwoQuality()
    {
        AssertItem(UpdateOnce(Brie, 0, 10), sellIn: -1, quality: 12);
    }

    [Theory]
    [InlineData(5, 50, 4, 50)]   // already capped
    [InlineData(0, 49, -1, 50)]  // past sell-by, would double-increment past 50
    public void AgedBrie_QualityNeverExceeds50(int sellIn, int quality, int expectedSellIn, int expectedQuality)
    {
        AssertItem(UpdateOnce(Brie, sellIn, quality), expectedSellIn, expectedQuality);
    }

    // ---- Sulfuras ----

    [Theory]
    [InlineData(0, 80)]
    [InlineData(-1, 80)]
    public void Sulfuras_NeverChangesSellInOrQuality(int sellIn, int quality)
    {
        AssertItem(UpdateOnce(Sulfuras, sellIn, quality), sellIn, quality);
    }

    // ---- Backstage passes ----

    [Theory]
    [InlineData(15, 20, 14, 21)]  // more than 10 days out: +1
    [InlineData(11, 20, 10, 21)]  // boundary: still +1
    [InlineData(10, 20, 9, 22)]   // boundary: now +2
    [InlineData(6, 20, 5, 22)]    // boundary: still +2
    [InlineData(5, 20, 4, 23)]    // boundary: now +3
    [InlineData(1, 20, 0, 23)]    // last day before the concert
    public void BackstagePasses_GainMoreQualityAsConcertApproaches(
        int sellIn, int quality, int expectedSellIn, int expectedQuality)
    {
        AssertItem(UpdateOnce(Backstage, sellIn, quality), expectedSellIn, expectedQuality);
    }

    [Fact]
    public void BackstagePasses_QualityNeverExceeds50()
    {
        AssertItem(UpdateOnce(Backstage, 5, 49), sellIn: 4, quality: 50);
    }

    [Fact]
    public void BackstagePasses_AfterConcert_QualityDropsToZero()
    {
        AssertItem(UpdateOnce(Backstage, 0, 20), sellIn: -1, quality: 0);
    }

    // ---- Conjured ----
    // Unlike the tests above, these assert the *spec*, not the original code.
    // The 2x decay rule was agreed and the golden master regenerated to match.

    [Fact]
    public void ConjuredItem_BeforeSellBy_LosesTwoQuality()
    {
        AssertItem(UpdateOnce(Conjured, 3, 6), sellIn: 2, quality: 4);
    }

    [Fact]
    public void ConjuredItem_PastSellBy_LosesFourQuality()
    {
        AssertItem(UpdateOnce(Conjured, 0, 6), sellIn: -1, quality: 2);
    }

    [Theory]
    [InlineData(5, 1, 4, 0)]   // before sell-by, would go below 0
    [InlineData(0, 3, -1, 0)]  // past sell-by, would go well below 0
    public void ConjuredItem_QualityNeverGoesNegative(int sellIn, int quality, int expectedSellIn, int expectedQuality)
    {
        AssertItem(UpdateOnce(Conjured, sellIn, quality), expectedSellIn, expectedQuality);
    }
}
