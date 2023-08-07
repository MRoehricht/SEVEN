<script lang="ts">
	import { page } from '$app/stores';

	import 'carbon-components-svelte/css/all.css';
	import { signIn, signOut } from '@auth/sveltekit/client';
	import {
		Header,
		SideNav,
		SideNavItems,
		SideNavMenu,
		SideNavMenuItem,
		SideNavLink,
		SideNavDivider,
		SkipToContent,
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
		Home,
		DocumentTasks,
		CloudDataOps,
		Diagram,
		Microservices_1,
		SettingsAdjust,
		UserAvatarFilledAlt,
		Logout
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

<Header company="SEVEN" platformName="Sandberg Electric Vehicle Eden Network" bind:isSideNavOpen>
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
			<SideNavLink icon={Home} text="Home" href="/" isSelected={path === '/'} />
			<SideNavLink
				icon={DocumentTasks}
				text="Rovertasks"
				href="/tasks"
				isSelected={path?.endsWith('/tasks')}
			/>
			<SideNavLink
				icon={Microservices_1}
				text="Sonden"
				href="/probes"
				isSelected={path?.endsWith('/probes')}
			/>
			<SideNavLink
				icon={CloudDataOps}
				text="Messdaten"
				href="/measurements"
				isSelected={path?.endsWith('/measurements')}
			/>
			<SideNavDivider />
			<SideNavMenu icon={Diagram} text="Diagramme">
				<SideNavMenuItem
					href="/diagramms/temperature"
					text="Temperatur"
					isSelected={path?.endsWith('/diagramms/temperature')}
				/>
			</SideNavMenu>
		</SideNavItems>
	</SideNav>
{/if}

<Content>
	<slot />
</Content>
