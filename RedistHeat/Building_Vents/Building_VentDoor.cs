using RimWorld;
using Verse;

namespace RedistHeat
{
	public class Building_VentDoor : Building_Door, ITempExchangable
	{
		public override void SpawnSetup()
		{
			base.SpawnSetup();
		}

		public override void Tick()
		{
			base.Tick();
			ExchangeHeat();
		}

		public virtual void ExchangeHeat()
		{
			if (Find.TickManager.TicksGame%ConstSet.E_INTERVAL_SEC != 0 || !Validate())
				return;
			var neighRooms = new Room[4];
			var neighRoomCount = 0;
			float totalTemp = 0;
			for (var i = 0; i < 4; i++)
			{
				var neigh = Position + GenAdj.CardinalDirections[i];

				if (!neigh.InBounds())
					continue;

				var r = neigh.GetRoom();
				if (r == null) continue;
				totalTemp += r.Temperature;
				neighRooms[neighRoomCount] = r;
				neighRoomCount++;
			}

			if (neighRoomCount == 0) return;

			var avgTemp = totalTemp / neighRoomCount;

			Position.GetRoom().Temperature = avgTemp;

			for (var i = 0; i < neighRoomCount; i++)
			{
				var neighTemp = neighRooms[i].Temperature;

				var diff = avgTemp - neighTemp;

				neighRooms[i].PushHeat(diff);
			}
		}

		public virtual bool Validate()
		{
			return !Locked;
		}
	}
}


