<script lang="ts">
	import type { Item } from '$lib';

	export let open = false;
	export let onClosed;
	export let selectedItem: Item;

	const modalClose = () => {
		open = false;
		if (onClosed) {
			onClosed();
		}
	};

</script>

{#if open}
	<div class="modal" id="sampleModal" tabindex="-1" role="dialog" aria-labelledby="sampleModalLabel"
			 aria-hidden={false}>
		<div class="modal-dialog" role="document">
			<div class="modal-content">
				<div class="modal-header">
					<h5 class="modal-title" id="sampleModalLabel">{selectedItem?.name}</h5>
					<button type="button" class="close" data-dismiss="modal" aria-label="Close" on:click={modalClose}>
						<span aria-hidden="true">&times;</span>
					</button>
				</div>
				<div class="modal-body">...</div>
				<div class="modal-footer">
					<button type="button" class="btn btn-secondary" data-dismiss="modal" on:click={modalClose}>
						Close
					</button>
					<button type="button" class="btn btn-primary">
						Save changes
					</button>
				</div>
			</div>
		</div>
	</div>
{/if}

<style>
    .modal {
        display: block;
    }
</style>