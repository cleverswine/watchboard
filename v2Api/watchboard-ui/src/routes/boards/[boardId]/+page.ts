import type { PageLoad } from './$types';
import { type Board, BoardList } from '$lib';

export const load: PageLoad = async ({ fetch, params }) => {
	const boardsResp = await fetch(`/api/boards/${params.boardId}`);
	const board: Board = await boardsResp.json();
	for (const l of board.lists) {
		const listResp = await fetch(`/api/lists/${l.id}`);
		const listDetail: BoardList = await listResp.json();
		l.items = listDetail.items;
	}
	return { board };
};