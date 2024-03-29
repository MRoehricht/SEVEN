import { SvelteKitAuth } from '@auth/sveltekit';
import GitHub from '@auth/core/providers/github';

import { env } from '$env/dynamic/private';
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
		providers: [GitHub({ clientId: env.GITHUB_ID, clientSecret: env.GITHUB_SECRET })]
	}),
	authorization
);

/*
import AzureADB2C from "@auth/core/providers/azure-ad-b2c";
const request = new Request("https://example.com");
const response = await AuthHandler(request, {
  // optionally, you can pass `tenantId` and `primaryUserFlow` instead of `issuer`
  providers: [AzureADB2C({ clientId: "", clientSecret: "", issuer: "" })],
});
*/