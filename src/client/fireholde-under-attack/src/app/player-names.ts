const ADJECTIVES = [
  'Ancient', 'Arcane', 'Amber', 'Bold', 'Brave',
  'Bronze', 'Calm', 'Clever', 'Cobalt', 'Crimson',
  'Daring', 'Dawn', 'Dusk', 'Elven', 'Ember',
  'Faithful', 'Gentle', 'Gilded', 'Golden', 'Grand',
  'Hearty', 'Hidden', 'Humble', 'Iron', 'Ivory',
  'Jade', 'Keen', 'Lofty', 'Lunar', 'Misty',
  'Mossy', 'Mystic', 'Noble', 'Oaken', 'Onyx',
  'Proud', 'Quick', 'Radiant', 'Rocky', 'Royal',
  'Silent', 'Silver', 'Solar', 'Stout', 'Swift',
  'Thorned', 'True', 'Valiant', 'Wandering', 'Wise',
];

const NOUNS = [
  'Archer', 'Alder', 'Bard', 'Birch', 'Brewer',
  'Cloak', 'Cleric', 'Drake', 'Druid', 'Elder',
  'Falcon', 'Fisher', 'Forge', 'Grove', 'Guard',
  'Harper', 'Herald', 'Hollow', 'Hunter', 'Keeper',
  'Knight', 'Lantern', 'Lancer', 'Mage', 'Mantle',
  'Mason', 'Miller', 'Mist', 'Monk', 'Oak',
  'Oracle', 'Pilgrim', 'Pine', 'Porter', 'Quest',
  'Ranger', 'Reed', 'Sage', 'Scout', 'Seer',
  'Shield', 'Smith', 'Sparrow', 'Stone', 'Storm',
  'Temple', 'Thorn', 'Tower', 'Wanderer', 'Weaver',
];

export function generatePlayerName(): string {
  const adj = ADJECTIVES[Math.floor(Math.random() * ADJECTIVES.length)];
  const noun = NOUNS[Math.floor(Math.random() * NOUNS.length)];
  return `${adj}${noun}`;
}
