

class EmailTemplate
{
    public static String GetBody(String FirstName, String ApplicationName)
    {
        String data = $"""

            We're thrilled to welcome you to the {ApplicationName} community!

            Get started with Our App:

            If you have any questions or need assistance, don't hesitate to reach out to our friendly support team:

            Email us at: Resources@AtmApp.gov.biz
            
            We're excited to have you on board and can't wait to see what you achieve with {ApplicationName}!

            Best regards,

            The {ApplicationName} Team
            """;

        return data;
    }

    public static String GetSubject(String FirstName, String ApplicationName)
    {
        String data = $"""Welcome to the {ApplicationName}, {FirstName}!""";

        return data;
    }
}
