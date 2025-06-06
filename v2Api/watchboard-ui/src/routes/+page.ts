import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch }) => {
	const boardsResp = await fetch(`/api/boards`);
	return { boards: await boardsResp.json() };
};