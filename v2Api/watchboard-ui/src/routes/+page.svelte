<script lang="ts">
	let { data } = $props();
</script>

<h1>Welcome to SvelteKit</h1>
<p>Visit <a href="https://svelte.dev/docs/kit">svelte.dev/docs/kit</a> to read the documentation</p>

{#each data.boards as board (board.id)}
	<ul>
		<li>
			<a href="/boards/{board.id}">{board.name}</a>
		</li>
	</ul>
{/each}