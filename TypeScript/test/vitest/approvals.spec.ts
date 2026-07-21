import { execSync } from 'node:child_process';
import { Item, GildedRose } from '@/gilded-rose';

/**
 * Snapshot-based approval tests. The "thirtyDays" case runs the same
 * program that produces the canonical golden master in
 * ../../texttests/ThirtyDays/stdout.gr (see scripts/golden-master-check.sh
 * at the repo root for the authoritative check).
 */

describe('Gilded Rose Approval', () => {
  it('should foo', () => {
    const gildedRose = new GildedRose([new Item('foo', 0, 0)]);
    const items = gildedRose.updateQuality();

    expect(items).toMatchSnapshot();
  });

  it('should thirtyDays', () => {
    const consoleOutput = execSync(
      'npx tsx test/golden-master-text-test.ts 30',
      { encoding: 'utf-8' }
    );

    expect(consoleOutput).toMatchSnapshot();
  });
});
