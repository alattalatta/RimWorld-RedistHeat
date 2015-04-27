using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class BuildingVentDoor : Building_Door
	{
		private const float EqualizationRate = 0.21f;
		private int curTick, elapTick;
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			curTick = elapTick = Find.TickManager.TicksGame;
		}

		public override void Tick()
		{
			base.Tick();
			curTick = Find.TickManager.TicksGame;
			if (curTick - elapTick <= 250) return;

			var roomNorth = (Position + IntVec3.North.RotatedBy(Rotation)).GetRoom();
			if (roomNorth == null) return;
			var roomSouth = (Position + IntVec3.South.RotatedBy(Rotation)).GetRoom();
			if (roomSouth == null) return;

			if (roomNorth == roomSouth || (roomNorth.UsesOutdoorTemperature && roomSouth.UsesOutdoorTemperature))
				return;

			float tempEq;
			if (roomNorth.UsesOutdoorTemperature)
				tempEq = roomNorth.Temperature;
			else if (roomSouth.UsesOutdoorTemperature)
				tempEq = roomSouth.Temperature;
			else
			{
				tempEq = (roomNorth.Temperature * roomNorth.CellCount + roomSouth.Temperature * roomSouth.CellCount)
					/ (roomNorth.CellCount + roomSouth.CellCount);
			}

			if (!roomNorth.UsesOutdoorTemperature)
				ExchangeHeat(roomNorth, tempEq, EqualizationRate);
			if (!roomSouth.UsesOutdoorTemperature)
				ExchangeHeat(roomSouth, tempEq, EqualizationRate);

			elapTick = Find.TickManager.TicksGame;
		}

		private static void ExchangeHeat(Room r, float targetTemp, float rate)
		{
			var tempDiff = Mathf.Abs(r.Temperature - targetTemp);
			var tempRated = tempDiff * rate;
			if (targetTemp < r.Temperature)
				r.Temperature = Mathf.Max(targetTemp, r.Temperature - tempRated);
			else if (targetTemp > r.Temperature)
				r.Temperature = Mathf.Min(targetTemp, r.Temperature + tempRated);
		}
	}
}


