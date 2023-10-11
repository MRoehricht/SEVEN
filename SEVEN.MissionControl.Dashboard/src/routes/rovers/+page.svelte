<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import CaretDown from 'carbon-icons-svelte/lib/CaretDown.svelte';
	import CaretLeft from 'carbon-icons-svelte/lib/CaretLeft.svelte';
	import CaretRight from 'carbon-icons-svelte/lib/CaretRight.svelte';
	import CaretUp from 'carbon-icons-svelte/lib/CaretUp.svelte';
	import StopOutline from 'carbon-icons-svelte/lib/StopOutline.svelte';
	import { Button, Grid, Row, Column, Slider } from 'carbon-components-svelte';
	import DashboardToolbar from '$lib/components/DashboardToolbar.svelte';

	let speed: number = 0;
	let socket: WebSocket;
	onMount(() => {
		socket = new WebSocket('ws://192.168.178.4:8745');
		socket.addEventListener('open', () => {
			console.log('Opened');
		});
	});

	onDestroy(() => {
		socket?.close();
	});
</script>

<DashboardToolbar
	title="Rover"
	crumbs={[
		{ label: 'Home', path: '/' },
		{ label: 'Rover', path: '/rovers' }
	]}
/>

<Grid narrow>
	<Row>
		<Column sm={1} md={1} lg={1} />
		<Column sm={1} md={1} lg={1}
			><Button
				tooltipPosition="top"
				tooltipAlignment="end"
				iconDescription="Vorwärts"
				size="lg"
				icon={CaretUp}
				on:click={() => socket?.send('1;0;' + speed + ';0;0')}
			/></Column
		>
		<Column sm={1} md={1} lg={1} />
	</Row>
	<Row>
		<Column sm={1} md={1} lg={1}
			><Button
				tooltipPosition="top"
				tooltipAlignment="end"
				iconDescription="Links"
				size="lg"
				icon={CaretLeft}
				on:click={() => socket?.send('0;1;0;0;' + speed + '')}
			/></Column
		>
		<Column sm={1} md={1} lg={1}>
			<Button
				tooltipPosition="top"
				tooltipAlignment="start"
				iconDescription="Stopp"
				size="lg"
				icon={StopOutline}
				on:click={() => socket?.send('0;0;0;0;0')}
			/></Column
		>
		<Column sm={1} md={1} lg={1}
			><Button
				tooltipPosition="top"
				tooltipAlignment="end"
				iconDescription="Rechts"
				size="lg"
				icon={CaretRight}
				on:click={() => socket?.send('0;2;0;' + speed + ';0')}
			/></Column
		>
	</Row>
	<Row>
		<Column sm={1} md={1} lg={1} />
		<Column sm={1} md={1} lg={1}
			><Button
				tooltipPosition="bottom"
				tooltipAlignment="end"
				iconDescription="Rückwärts"
				size="lg"
				icon={CaretDown}
				on:click={() => socket?.send('2;0;' + speed + ';0;0')}
			/></Column
		>
		<Column sm={1} md={1} lg={1} />
	</Row>
</Grid>

<Slider
	light
	labelText="Geschwíndigkeit"
	min={55000}
	max={65520}
	bind:value={speed}
	step={1000}
	hideTextInput
/>
