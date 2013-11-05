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
	Checks if a data change subscription should be fired on a date
	If so, Inserts the necessary row in the Notification table and its child rows
*/

-- TODO: Remove this stored procedure

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'InsertChangeNotificationData')
	BEGIN
		DROP  Procedure  InsertChangeNotificationData
	END

GO

CREATE Procedure InsertChangeNotificationData
(
	@SubscriptionId UNIQUEIDENTIFIER,
	@Today DATETIME,
	@LastTime DATETIME
)

AS
	DECLARE @NotificationId UNIQUEIDENTIFIER
	DECLARE @IsForAllPersons BIT
	
	-- Select subscription data
	SELECT @IsForAllPersons= IsForAllPersons
	FROM Subscription AS S 
	INNER JOIN DataSubscription AS DS ON S.SubscriptionId = DS.SubscriptionId
	WHERE S.SubscriptionId = @SubscriptionId
	
	-- Use this table to store temporary data
	CREATE TABLE #Person (NotificationPersonId UNIQUEIDENTIFIER DEFAULT NEWID(), PersonUuid UNIQUEIDENTIFIER)
	
	-- Search for persons that could possible fire the notification
	INSERT INTO #Person (PersonUuid)
	SELECT P.PersonUuid
	FROM Person AS P
	LEFT OUTER JOIN SubscriptionPerson AS SP ON P.PersonUuid = SP.PersonUuid
	WHERE 
	(
		@IsForAllPersons = 1 OR SP.SubscriptionId = @SubscriptionId
	)
	AND
	(
		dbo.IsDataChangeEvent(P.PersonUuid, @Today, @LastTime) = 1
	)
	
	
	IF EXISTS (SELECT TOP 1 * FROM #Person) -- If persons are found
	BEGIN
		-- Insert data into physical tables
		SET @NotificationID = NEWID()
		
		INSERT INTO Notification (NotificationId, SubscriptionId, NotificationDate)
		VALUES (@NotificationId, @SubscriptionId, @Today)
		
		INSERT INTO NotificationPerson (NotificationId, NotificationPersonId, PersonUuid)
		SELECT @NotificationId, NotificationPersonId, PersonUuid
		FROM #Person		
		
	END
	
	-- Finally, return the new Notification object
	SELECT * 
	FROM Notification 
	WHERE NotificationId = @NotificationId
GO