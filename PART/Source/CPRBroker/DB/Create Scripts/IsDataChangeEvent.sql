/*
	Determines if a person's data has changed in the given perion
*/
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'FN' AND name = 'IsDataChangeEvent')
	BEGIN
		DROP  FUNCTION IsDataChangeEvent
	END

GO

CREATE FUNCTION IsDataChangeEvent
(
	@PersonId UNIQUEIDENTIFIER,
	@Now DATETIME,
	@LastTime DATETIME
)
RETURNS BIT
AS
	BEGIN
		DECLARE @RET BIT
		SET @RET = 0
		
		IF EXISTS
			( 
				-- If data has changed in Person table
				SELECT PersonHistoryId 
				FROM PersonHistory AS PH
				WHERE 
					PH.PersonId = @PersonId
				AND	PH.ChangeDate < @Now
				AND PH.ChangeDate >= @LastTime 
			)
		OR 
			EXISTS
			( 
				-- If data has changed in Address table
				SELECT AddressHistoryId 
				FROM AddressHistory AS AH
				WHERE 
					AH.PersonId = @PersonId
				AND	AH.ChangeDate < @Now
				AND AH.ChangeDate >= @LastTime 
			)
		OR
			EXISTS
			(
				-- If data has changed in Relationship table
				SELECT RelationshipHistoryId 
				FROM RelationshipHistory AS RH
				WHERE 
					RH.PersonId = @PersonId
				AND	RH.ChangeDate < @Now
				AND RH.ChangeDate >= @LastTime 
			)
		OR 
			EXISTS
			(
				-- if data has changed in TimedRelationship table
				SELECT TimedRelationshipHistoryId 
				FROM TimedRelationshipHistory AS TRH
				INNER JOIN RelationshipHistory AS RH ON TRH.RelationshipId = RH.RelationshipId
				WHERE 
					RH.PersonId = @PersonId
				AND	TRH.ChangeDate < @Now
				AND TRH.ChangeDate >= @LastTime 
			)
		OR
			EXISTS
			(
				-- if data has changed in MaritalRelationship table
				SELECT MaritalRelationshipHistoryId 
				FROM MaritalRelationshipHistory AS MRH
				INNER JOIN RelationshipHistory AS RH ON MRH.RelationshipId = RH.RelationshipId
				WHERE 
					RH.PersonId = @PersonId
				AND	MRH.ChangeDate < @Now
				AND MRH.ChangeDate >= @LastTime 
			)
			SET @RET = 1
		RETURN @RET
	END

GO
