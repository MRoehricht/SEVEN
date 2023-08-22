<script lang="ts">
	import type { DqlToken } from '$lib/types';

	type DqlTokenGroup = {
		type: 'form' | 'logical';
		tokens: DqlToken[];
	};

	let whereTokens: DqlTokenGroup[] = [];

	function addDefaultWhereTokens() {
		let newTokens = [] as DqlTokenGroup[];
		const defaultTokens = {
			type: 'form',
			tokens: [
				{
					type: 'property',
					value: 'id',
					index: 0
				},
				{
					type: 'operator',
					value: '==',
					index: 0
				},
				{
					type: 'value',
					value: '123',
					index: 0
				}
			]
		} as DqlTokenGroup;

		if (whereTokens.length === 0) {
			newTokens.push(defaultTokens);
		} else {
			newTokens.push({
				type: 'logical',
				tokens: [
					{
						type: 'operator',
						value: 'AND',
						index: 0
					}
				]
			});
			newTokens.push(defaultTokens);
		}

		whereTokens = [...whereTokens, ...newTokens];
	}

	$: console.log(whereTokens);
</script>

<div>
	<div class="field-row">
		<span class="field-row-label">FROM</span>
		<button class="field-row-button">measurements</button>
		<span class="field-row-label">WHERE</span>
		{#each whereTokens as token}
			{#if token.type == 'logical'}
				<button class="field-row-button">{token.tokens[0].value}</button>
			{:else if token.type == 'form'}
				<div class="field-row-form">
					{#each token.tokens as subToken}
						{#if subToken.type == 'property'}
							<button class="field-row-button">{subToken.value}</button>
						{:else if subToken.type == 'operator'}
							<button class="field-row-button">{subToken.value}</button>
						{:else if subToken.type == 'value'}
							<button class="field-row-button">{subToken.value}</button>
						{/if}
					{/each}
				</div>
			{/if}
		{/each}
		<button class="field-row-button" on:click={addDefaultWhereTokens}>+</button>
		<span class="field-row-field" />
	</div>
</div>

<style>
	.field-row {
		display: flex;
		flex-flow: wrap;
		align-content: flex-start;
		row-gap: 8px;
	}

	.field-row-label,
	.field-row-button,
	.field-row-field {
		display: flex;
		align-items: center;
		justify-content: space-between;
		flex: 1;
		flex-shrink: 0;
		padding: 0.75rem;
		background-color: white;
		border-radius: 2px;
		border: medium;
		margin-right: 4px;
	}

	.field-row-label {
		color: rgb(110, 159, 255);
		flex: 0;
	}

	.field-row-button {
		color: rgb(204, 204, 220);
		cursor: pointer;
		width: auto;
		flex: 0;
	}

	.field-row-form {
		align-items: flex-start;
		display: flex;
		flex-direction: row;
		position: relative;
		text-align: left;
	}
</style>
