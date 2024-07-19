package main

import (
	"fmt"
	"log"
	"time"

	"github.com/golang-jwt/jwt/v5"
	"github.com/google/uuid"
)

func main() {
	// Replace the example values below (remove the brackets).
	// Store secrets securely based on your team's best practices.
	// See: https://help.tableau.com/current/online/en-us/connected_apps_direct.htm

	secretID := "[Tableau Connected App Direct Trust Secret ID]"
	secretValue := "[Tableau Connected App Direct Trust Secret Value]"
	clientID := "[Tableau Connected App Direct Trust Client ID]"
	username := "[Tableau Username]"
	tokenExpiryInMinutes := 1 * time.Minute // Max of 10 minutes.
	expiry := time.Now().Add(tokenExpiryInMinutes)
	// Remove 'embed_authoring' scope if Authoring is not needed.
	scopes := []string{"tableau:views:embed", "tableau:views:embed_authoring"}

	uniqueID, err := uuid.NewUUID()
	if err != nil {
		log.Fatalln(err)
	}

	type UserClaims struct {
		Scope []string `json:"scp"`
		jwt.RegisteredClaims
	}
	claimSet :=
		UserClaims{
			scopes,
			jwt.RegisteredClaims{
				Issuer:    clientID,
				Subject:   username,
				ExpiresAt: jwt.NewNumericDate(expiry),
				IssuedAt:  jwt.NewNumericDate(time.Now()),
				Audience:  []string{"tableau"},
				ID:        uniqueID.String(),
			},
		}

	signer := []byte(secretValue)

	jwtToken := jwt.NewWithClaims(jwt.SigningMethodHS256, claimSet)
	// Set header
	jwtToken.Header["kid"] = secretID
	jwtToken.Header["iss"] = clientID

	jwtValue, err := jwtToken.SignedString(signer)
	if err != nil {
		log.Fatalln(err)
	}

	fmt.Println("Generating JWT...")
	fmt.Println(jwtValue)
}
