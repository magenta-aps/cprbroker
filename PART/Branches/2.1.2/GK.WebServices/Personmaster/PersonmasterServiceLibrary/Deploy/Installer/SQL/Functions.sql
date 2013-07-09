if exists (select 1
          from sysobjects
          where  id = object_id('fnGK_CORE_DatetimeMaxValue')
          and type in ('IF', 'FN', 'TF'))
   drop function fnGK_CORE_DatetimeMaxValue
go

if exists (select 1
          from sysobjects
          where  id = object_id('fnGK_CORE_DatetimeMinValue')
          and type in ('IF', 'FN', 'TF'))
   drop function fnGK_CORE_DatetimeMinValue
go

if exists (select 1
          from sysobjects
          where  id = object_id('fnGK_CORE_GetConfigValue')
          and type in ('IF', 'FN', 'TF'))
   drop function fnGK_CORE_GetConfigValue
go

if exists (select 1
          from sysobjects
          where  id = object_id('fnGK_CORE_GetStringFromGUID')
          and type in ('IF', 'FN', 'TF'))
   drop function fnGK_CORE_GetStringFromGUID
go


CREATE FUNCTION fnGK_CORE_DatetimeMaxValue ()
RETURNS datetime
BEGIN
	DECLARE @Result datetime
	SET     @Result = '9999-12-31 23:59:59'
	RETURN  @Result
END
go


CREATE FUNCTION fnGK_CORE_DatetimeMinValue ()
RETURNS datetime
BEGIN
	DECLARE @Result datetime
	SET     @Result = '1753-01-01 00:00:00'
	RETURN  @Result
END
go


CREATE FUNCTION fnGK_CORE_GetConfigValue (@name VARCHAR(60))
RETURNS VARCHAR(1020)
BEGIN
    DECLARE @result_value VARCHAR(1020)
    
    SET @result_value = ''
    
    IF @name IS NULL SET @name = ''
    SET @name = LTRIM( RTRIM( @name ) )
    
    SELECT  @result_value = cfg.value
    FROM    T_CORE_Config cfg
    WHERE   cfg.name = @name
    
    IF @result_value IS NULL SET @result_value = ''
    SET @result_value = LTRIM( RTRIM( @result_value ) )
    
    RETURN @result_value
END
go


CREATE FUNCTION fnGK_CORE_GetStringFromGUID (@arg1 uniqueidentifier)
RETURNS VARCHAR(38)
BEGIN
    IF @arg1 IS NULL RETURN ''
    RETURN CONVERT(VARCHAR(38), @arg1)
END
go