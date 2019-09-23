using System.Collections;
using Improbable.Gdk.Core;
using Improbable.Gdk.Health;
using Improbable.Gdk.Subscriptions;
using Pickups;
using UnityEngine;

namespace Fps
{
	[WorkerType(WorkerUtils.UnityGameLogic)]
	public class HealthPickupServerBehaviour : MonoBehaviour
	{
		[Require] private HealthPickupWriter           healthPickupWriter;
		[Require] private HealthComponentCommandSender healthCommandRequestSender;


		private Coroutine respawnCoroutine;

		private void OnEnable()
		{
			// If the pickup is inactive on initial checkout - turn off collisions and start the respawning process.
			if (!healthPickupWriter.Data.IsActive)
			{
				respawnCoroutine = StartCoroutine(RespawnHealthPackRoutine());
			}
		}

		private void OnDisable()
		{
			if (respawnCoroutine != null)
			{
				StopCoroutine(respawnCoroutine);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			// OnTriggerEnter is fired regardless of whether the MonoBehaviour is enabled/disabled.
			if (healthPickupWriter == null)
			{
				return;
			}

			if (!other.CompareTag("Player"))
			{
				return;
			}

			var health = other.gameObject.GetComponent< VisibilityAndCollision >()?.health;
			if (health != null)
			{
				if (health.Data.Health < health.Data.MaxHealth) HandleCollisionWithPlayer(other.gameObject);
			}
		}

		private void SetIsActive(bool isActive)
		{
			healthPickupWriter?.SendUpdate(new HealthPickup.Update { IsActive = new Option<bool>(isActive) });
		}

		private void HandleCollisionWithPlayer(GameObject player)
		{
			var playerSpatialOsComponent = player.GetComponent<LinkedEntityComponent>();

			if (playerSpatialOsComponent == null)
			{
				return;
			}


			healthCommandRequestSender.SendModifyHealthCommand(playerSpatialOsComponent.EntityId,
				new HealthModifier { Amount = healthPickupWriter.Data.HealthValue });

			// Toggle health pack to its "consumed" state
			SetIsActive(false);

			// Begin cool-down period before re-activating health pack
			respawnCoroutine = StartCoroutine(RespawnHealthPackRoutine());
		}

		private IEnumerator RespawnHealthPackRoutine()
		{
			yield return new WaitForSeconds(15f);
			SetIsActive(true);
		}
	}
}
