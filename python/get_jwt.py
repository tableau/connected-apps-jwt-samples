import datetime
import uuid
import jwt

# Replace the example values below (remove the brackets).
# Store secrets securely based on your team's best practices.
# See: https://help.tableau.com/current/online/en-us/connected_apps_direct.htm

secretId = "[Tableau Connected App Direct Trust Secret ID]"
secretValue = "[Tableau Connected App Direct Trust Secret Value]"
clientId = "[Tableau Connected App Direct Trust Client ID]"
username = "[Tableau Username]"
tokenExpiryInMinutes = 1  # Max of 10 minutes.

# Remove 'embed_authoring' scope if Authoring is not needed.
scopes = ["tableau:views:embed", "tableau:views:embed_authoring"]

kid = secretId
iss = clientId
sub = username
aud = "tableau"
exp = datetime.datetime.utcnow() + datetime.timedelta(minutes=tokenExpiryInMinutes)
jti = str(uuid.uuid4())
scp = scopes

userAttributes = {
    # User attributes are optional.
    # Add entries to this dictionary if desired.
    # "[User Attribute Name]": "[User Attribute Value]",
}

payload = {
    "iss": clientId,
    "exp": exp,
    "jti": jti,
    "aud": aud,
    "sub": sub,
    "scp": scp,
} | userAttributes

def getJwt():
    token = jwt.encode(
        payload,
        secretValue,
        algorithm="HS256",
        headers={
            "kid": kid,
            "iss": iss,
        },
    )


    return token


if __name__ == "__main__":
    token = getJwt()
    print(token)
