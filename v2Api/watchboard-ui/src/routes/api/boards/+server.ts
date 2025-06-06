import { WATCHBOARD_API } from '$env/static/private';

export async function GET() {
	return await fetch(`${WATCHBOARD_API}/boards`);
}
