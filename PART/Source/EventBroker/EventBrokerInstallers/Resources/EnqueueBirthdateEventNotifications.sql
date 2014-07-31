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
	Checks whether a birthdate subscription should fire a notification
	If so, creates a new row in EventNotification table with its child rows
*/
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'EnqueueBirthdateEventNotifications')
	BEGIN
		DROP  Procedure  EnqueueBirthdateEventNotifications
	END

GO

CREATE Procedure EnqueueBirthdateEventNotifications
(
	@SubscriptionId uniqueidentifier,
	@Today DateTime
)

AS
	
	DECLARE @NotificationId UNIQUEIDENTIFIER
	
	DECLARE @AgeYears INT
	DECLARE @PriorDays INT
	DECLARE @IsForAllPersons BIT
	
	-- Get subscription parameters
	SELECT @AgeYears = AgeYears, @PriorDays=PriorDays, @IsForAllPersons= IsForAllPersons
	FROM Subscription AS S 
	INNER JOIN BirthdateSubscription AS BDS ON S.SubscriptionId = BDS.SubscriptionId
	WHERE S.SubscriptionId = @SubscriptionId
	
	-- Temp table to hold persons
	CREATE TABLE #Person (EventNotificationId UNIQUEIDENTIFIER DEFAULT NEWID(), PersonUuid UNIQUEIDENTIFIER, Birthdate DATETIME, Age INT)
		
	
	-- Search  for persons that match the subscription rule
	INSERT INTO #Person (PersonUuid, Birthdate, Age)
	SELECT PB.PersonUuid, PB.BirthDate, DATEDIFF(YEAR, PB.Birthdate, DATEADD(day, @PriorDays, @Today))	
	FROM PersonBirthdate AS PB
	LEFT OUTER JOIN SubscriptionPerson AS SP ON PB.PersonUuid = SP.PersonUuid
	WHERE 
	(
		@IsForAllPersons = 1 OR SP.SubscriptionId = @SubscriptionId
	)
	AND
	(
		dbo.IsBirthdateEvent(@Today, PB.BirthDate, @AgeYears, @PriorDays) = 1
	)
	
	
	INSERT INTO EventNotification (EventNotificationId, SubscriptionId, PersonUUID, CreatedDate)
	SELECT EventNotificationId, @SubscriptionId, PersonUuid, @Today
	FROM #Person
		
	INSERT INTO BirthdateEventNotification (EventNotificationId, Age)
	SELECT EventNotificationId, Age
	FROM #Person
		
	

GO


