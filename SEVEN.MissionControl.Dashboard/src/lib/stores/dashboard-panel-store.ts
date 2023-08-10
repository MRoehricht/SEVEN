import type { GridItemProps } from 'svelte-grid-extended/GridItem.svelte';
import { writable } from 'svelte/store';

export const panels = writable<PanelProps[]>([]);

export type PanelProps = {
	id: string;
	title: string;
	probeId: string;
	measurementType: number;
	gridItem: GridItemProps;
};
