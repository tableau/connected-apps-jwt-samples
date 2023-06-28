from flask import Flask, json

import datetime
import uuid
import jwt

app = Flask(__name__)

# Replace the example values below (remove the brackets).
# Store secrets securely based on your team's best practices.
# See: https://help.tableau.com/current/online/en-us/connected_apps_direct.htm

secretId = "[Connected App Secret ID]"
secretValue = "[Connected App Secret Value]"
clientId = "[Connected App Client ID]"
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


@app.after_request
def after_request(response):
    response.headers[
        "Cache-Control"
    ] = "no-cache, no-store, must-revalidate, public, max-age=0"
    response.headers["Expires"] = 0
    response.headers["Pragma"] = "no-cache"
    response.headers["Content-Type"] = "application/json"
    return response


@app.route("/token", methods=["GET"])
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

    headers = {"Content-Type": "application/json"}

    return json.dumps({"jwt": token})


if __name__ == "__main__":
    app.run()
