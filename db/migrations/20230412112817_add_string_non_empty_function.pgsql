CREATE FUNCTION string_non_empty(string text) RETURNS bool
    LANGUAGE SQL
    IMMUTABLE
    RETURNS NULL ON NULL INPUT
    RETURN ((string)::text = ''::text) IS NOT TRUE;
