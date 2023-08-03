import { SvelteKitAuth } from '@auth/sveltekit';
import GitHub from '@auth/core/providers/github';
import { GITHUB_ID, GITHUB_SECRET } from '$env/static/private';
import { redirect, type Handle } from '@sveltejs/kit';
import { sequence } from '@sveltejs/kit/hooks';

export const authorization = (async ({ event, resolve }) => {
	// Protect any routes except /login
	if (
		event.url.pathname.startsWith('/') &&
		event.url.pathname !== '/login' &&
		!event.url.pathname.startsWith('/auth')
	) {
		const session = await event.locals.getSession();
		if (!session) {
			throw redirect(303, '/login');
		}
	}

	// If the request is still here, just proceed as normally
	return resolve(event);
}) satisfies Handle;

// First handle authentication, then authorization
// Each function acts as a middleware, receiving the request handle
// And returning a handle which gets passed to the next function
export const handle: Handle = sequence(
	SvelteKitAuth({
		providers: [GitHub({ clientId: GITHUB_ID, clientSecret: GITHUB_SECRET })]
	}),
	authorization
);
