/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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
	@PersonUuid UNIQUEIDENTIFIER,
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
					PH.PersonUuid = @PersonUuid
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
					AH.PersonUuid = @PersonUuid
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
					RH.PersonUuid = @PersonUuid
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
					RH.PersonUuid = @PersonUuid
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
					RH.PersonUuid = @PersonUuid
				AND	MRH.ChangeDate < @Now
				AND MRH.ChangeDate >= @LastTime 
			)
			SET @RET = 1
		RETURN @RET
	END

GO
