<script lang="ts">
	import { page } from '$app/stores';
	import { version } from '$app/environment';
	import 'carbon-components-svelte/css/all.css';
	import { signOut } from '@auth/sveltekit/client';
	import {
		Header,
		SideNav,
		SideNavItems,
		SideNavLink,
		Content,
		Theme,
		HeaderAction,
		HeaderGlobalAction,
		HeaderPanelDivider,
		HeaderPanelLink,
		HeaderPanelLinks,
		HeaderUtilities,
		HeaderActionLink
	} from 'carbon-components-svelte';
	import {
		DocumentTasks,
		Microservices_1,
		UserAvatarFilledAlt,
		Dashboard
	} from 'carbon-icons-svelte';

	import '@carbon/styles/css/styles.css';
	import '@carbon/charts-svelte/styles.css';
	import type { CarbonTheme } from 'carbon-components-svelte/types/Theme/Theme.svelte';

	let isSideNavOpen = false;
	let isAccountOpen = false;
	let theme: CarbonTheme = 'g10';
	let path: string;
	let unsubscribe = page.subscribe((value) => {
		path = value.route.id ?? '/';
	});

	export function onDestroy() {
		unsubscribe();
	}
</script>

<Theme bind:theme />
<Header company="SEVEN" platformName="Sandberg Electric Vehicle Eden Network - v{version}" bind:isSideNavOpen>
	{#if $page.data.session}
		<HeaderUtilities>
			<HeaderAction
				bind:isOpen={isAccountOpen}
				icon={UserAvatarFilledAlt}
				closeIcon={UserAvatarFilledAlt}
			>
				<HeaderPanelLinks>
					<HeaderPanelDivider>Angemeldet als: {$page.data.session.user?.name}</HeaderPanelDivider>
					<HeaderPanelLink on:click={() => signOut()}>Abmelden</HeaderPanelLink>
				</HeaderPanelLinks>
			</HeaderAction>
		</HeaderUtilities>
	{/if}
</Header>

{#if $page.data.session}
	<SideNav bind:isOpen={isSideNavOpen} rail>
		<SideNavItems>
			<SideNavLink icon={Dashboard} text="Dashboard" href="/" isSelected={path === '/'} />
			<!--<SideNavLink
				icon={DocumentTasks}
				text="Rovertasks"
				href="/tasks"
				isSelected={path?.endsWith('/tasks')}
			/>-->
			<SideNavLink
				icon={Microservices_1}
				text="Sonden"
				href="/probes"
				isSelected={path?.endsWith('/probes')}
			/>
		</SideNavItems>
	</SideNav>
{/if}

<Content>
	<slot />
</Content>
