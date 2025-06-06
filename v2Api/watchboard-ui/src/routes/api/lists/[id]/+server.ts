import { WATCHBOARD_API } from '$env/static/private';

export async function GET({ params }) {
	return await fetch(`${WATCHBOARD_API}/lists/${params.id}`);
}