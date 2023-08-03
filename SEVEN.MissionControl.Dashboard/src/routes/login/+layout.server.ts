import { redirect } from '@sveltejs/kit';
import type { LayoutServerLoad } from './$types';

export const load = (async ({ locals }) => {
	let session = await locals.getSession();
	if (session?.user) {
		throw redirect(307, '/');
	}
}) satisfies LayoutServerLoad;
