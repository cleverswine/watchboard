import { WATCHBOARD_API } from '$env/static/private';

export async function GET({ params, url }) {
	return await fetch(`${WATCHBOARD_API}/items/${params.id}${url.searchParams.get('a')}`);
}

export async function PUT({ params, url }) {
	return await fetch(`${WATCHBOARD_API}/items/${params.id}${url.searchParams.get('a')}`, {
		method: 'PUT'
	});
}

export async function DELETE({ params }) {
	return await fetch(`${WATCHBOARD_API}/items/${params.id}`, { method: 'DELETE' });
}
