import { Item, GildedRose } from '@/gilded-rose';

describe('Gilded Rose', () => {
  it('keeps the item name unchanged', () => {
    const gildedRose = new GildedRose([new Item('foo', 0, 0)]);
    const items = gildedRose.updateQuality();
    expect(items[0].name).toBe('foo');
  });
});
