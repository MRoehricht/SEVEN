<script>
	import { signIn, signOut } from '@auth/sveltekit/client';
	import { page } from '$app/stores';
	import { Button } from 'carbon-components-svelte';
	import { LogoGithub, Logout } from 'carbon-icons-svelte';
</script>

<div class="centered-div">
	{#if !$page.data.session}
		<div>
			<p>Du bist nicht angemeldet!</p>
			<p>Bitte melde dich mit einem der verfügbaren Providern an:</p>
		</div>
		<Button icon={LogoGithub} on:click={() => signIn('github')}>Github</Button>
	{/if}
</div>

<style>
	.centered-div {
		display: flex;
		flex-direction: column;
		justify-content: center;
		align-items: center;
		gap: 1rem;
	}

	.centered-div div > p {
		text-align: center;
	}
</style>
