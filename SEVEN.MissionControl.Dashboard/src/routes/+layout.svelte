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
		HeaderNavMenu,
		HeaderNavItem,
		HeaderAction,
		HeaderGlobalAction,
		HeaderPanelDivider,
		HeaderPanelLink,
		HeaderPanelLinks,
		HeaderUtilities,
		HeaderActionLink,
		SideNavMenu,
		SideNavMenuItem,
		SideNavDivider,
		SkipToContent,
		Checkbox
	} from 'carbon-components-svelte';
	import {
		EdgeDevice,
		DocumentTasks,
		Microservices_1,
		UserAvatarFilledAlt,
		AgricultureAnalytics,
		TouchInteraction,
		Query,
		Chip,
		Explore,
		LogoGithub,
		SettingsAdjust,
		Label,
		DroneVideo
	} from 'carbon-icons-svelte';
	import { onMount } from 'svelte';
	import '@carbon/styles/css/styles.css';
	import '@carbon/charts-svelte/styles.css';
	import type { CarbonTheme } from 'carbon-components-svelte/types/Theme/Theme.svelte';
	import type { Probe } from '$lib/types';
	import { DeviceService } from '$lib/utils/deviceService';

	let isSideNavOpen = false;
	let isAccountOpen = false;
	let isExpanded = false;
	let isRail = true;
	let isSettingsOpen = false;
	let theme: CarbonTheme = 'g10';
	let path: string;
	let unsubscribe = page.subscribe((value) => {
		path = value.route.id ?? '/';
	});
	let probes: Probe[] = [];
	let isLoading = true;
	onMount(() => {
		fetchDevices();
		document.addEventListener('navigationUpdated', fetchDevices);

		if (!localStorage.getItem('isRail')) {
			localStorage.setItem('isRail', JSON.stringify(isRail));
		} else {
			isRail = JSON.parse(localStorage.getItem('isRail') as string);
		}
	});

	async function fetchDevices() {
		isLoading = true;
		probes = await DeviceService.fetchDevices();
		isLoading = false;
	}

	function toggleIsRail() {
		localStorage.setItem('isRail', JSON.stringify(isRail));
	}

	export function onDestroy() {
		unsubscribe();
		document.removeEventListener('navigationUpdated', fetchDevices);
	}
</script>

<Theme bind:theme />
<Header
	company="SEVEN"
	platformName="Sandberg Electric Vehicle Eden Network - v{version}"
	bind:isSideNavOpen
>
	<svelte:fragment slot="skip-to-content">
		<SkipToContent />
	</svelte:fragment>

	{#if $page.data.session}
		<HeaderUtilities>
			<HeaderAction aria-label="Einstellungen" icon={SettingsAdjust} bind:isOpen={isSettingsOpen}>
				<HeaderPanelLinks>
					<HeaderPanelDivider>Menüeinstellungen:</HeaderPanelDivider>
					<Checkbox labelText="immer einklappen" bind:checked={isRail} on:change={toggleIsRail} />
				</HeaderPanelLinks>
			</HeaderAction>
			<HeaderActionLink
				href="https://github.com/MRoehricht/SEVEN"
				icon={LogoGithub}
				target="_blank"
			/>
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
	{:else}
		<HeaderActionLink
			href="https://github.com/MRoehricht/SEVEN"
			icon={LogoGithub}
			target="_blank"
		/>
	{/if}
</Header>

<SideNav bind:isOpen={isSideNavOpen} rail={isRail}>
	{#if $page.data.session}
		<SideNavItems>
			<SideNavLink
				icon={AgricultureAnalytics}
				text="Dashboard"
				href="/"
				isSelected={path === '/'}
			/>
			<SideNavLink
				icon={Microservices_1}
				text="Sonden"
				href="/probes"
				isSelected={path?.endsWith('/probes')}
			/>
			<SideNavLink
				icon={TouchInteraction}
				text="Aktoren"
				href="/actuators"
				isSelected={path?.endsWith('/actuators')}
			/>
			<SideNavDivider />
			<SideNavLink
				icon={Chip}
				text="Geräteübersicht"
				href="/devices"
				isSelected={path?.endsWith('/devices')}
			/>
			{#if !isLoading && probes.length > 0}
				<SideNavMenu text="Geräte" icon={EdgeDevice} bind:expanded={isExpanded}>
					{#each probes as probe}
						<SideNavMenuItem
							text={probe.name}
							href="/devices/{probe.id}"
							isSelected={path?.endsWith('/devices/' + probe.id)}
						/>
					{/each}
				</SideNavMenu>
			{/if}
			<SideNavLink
				icon={Explore}
				text="Explore"
				href="/devices/explore"
				isSelected={path?.endsWith('/devices/explore')}
			/>
			<SideNavLink
				icon={DroneVideo}
				text="Rover"
				href="/rovers"
				isSelected={path?.endsWith('/rovers')}
			/>
		</SideNavItems>
	{/if}
</SideNav>

<Content>
	<slot />
</Content>
