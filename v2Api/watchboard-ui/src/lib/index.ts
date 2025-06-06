// place files you want to import through the `$lib` alias in this folder.
export class OrderedItem {
	id: string = '';
	name: string = '';
	order: number = 0;
}

export class Board extends OrderedItem {
	lists: BoardList[] = [];
}

export class BoardList extends OrderedItem {
	default: boolean = false;
	boardId: string = '';
	items: Item[] = [];
}

export enum ItemType {
	Movie,
	TvShow
}

export class Item extends OrderedItem {
	tmDbId: number = 0;
	itemType: ItemType = ItemType.TvShow;
	releaseDate: string | null = null;
	notes?: string;
	language?: string;
	watchProvider?: string;
	boardListId: string = '';
}
