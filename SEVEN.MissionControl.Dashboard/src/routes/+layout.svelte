<script lang="ts">
	import 'carbon-components-svelte/css/all.css';

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
		Theme
	} from 'carbon-components-svelte';
	import { Home, DocumentTasks, CloudDataOps, Diagram } from 'carbon-icons-svelte';

	import '@carbon/styles/css/styles.css';
	import '@carbon/charts-svelte/styles.css';
	import type { CarbonTheme } from 'carbon-components-svelte/types/Theme/Theme.svelte';

	let isSideNavOpen = false;
	let theme: CarbonTheme = 'g10';
</script>

<Theme bind:theme />

<Header company="SEVEN" platformName="Sandberg Electric Vehicle Eden Network" bind:isSideNavOpen>
	<!-- <svelte:fragment slot="skip-to-content">
		<SkipToContent />
	</svelte:fragment> -->
</Header>

<SideNav bind:isOpen={isSideNavOpen} rail>
	<SideNavItems>
		<SideNavLink icon={Home} text="Home" href="/" isSelected />
		<SideNavLink icon={DocumentTasks} text="Rovertasks" href="/" />
		<SideNavLink icon={CloudDataOps} text="Messdaten" href="/measurements" />
		<SideNavDivider />
		<SideNavMenu icon={Diagram} text="Diagramme">
			<SideNavMenuItem href="/" text="Temperatur" />
		</SideNavMenu>
	</SideNavItems>
</SideNav>

<Content>
	<slot />
</Content>
