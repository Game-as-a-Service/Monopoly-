namespace Monopoly.DomainLayer.Domain.Maps;

public class FiveXNineMap : Map
{
	public FiveXNineMap() : base("5x9", Blocks)
	{
	}
	
	private static Block?[][] Blocks =>
		new Block?[][]
		{
			new Block?[] { new StartPoint("Start"),	new Land("A1", lot:"A"),	new Land("A2", lot:"A"),	new Station("Station1"),	new Land("B1", lot:"B"),	new Land("B2", lot:"B"),	new Land("B3", lot:"B"),		null,						null},
			new Block?[] { new Land("H1", lot:"H"), null,                    	null,                       null,    					null,                   	null,						new Road("R1"),    	  			null,   					null},
			new Block?[] { new Station("Station4"), null,       			 	null,               		new Land("D1", lot:"D"),    new Land("D2", lot:"D"),	new Land("D3", lot:"D"),    new ParkingLot("ParkingLot"),	new Land("C1", lot:"C"),	new Land("C2", lot:"C")},
			new Block?[] { new Land("H2", lot:"H"), new Land("G1", lot:"G"), 	new Land("G2", lot:"G"),    new Jail("Jail"),   		null,    					null,						null,							null,     					new Station("Station2")},
			new Block?[] { null, 					null,					 	null,						new Land("F1", lot:"F"), 	new Land("F2", lot:"F"),    new Station("Station3"),    new Land("E1", lot:"E"),    	new Land("E2", lot:"E"),   	new Road("R2")},
		};
}