import java.util.Calendar;
import java.util.Date;
import java.util.UUID;

import com.nimbusds.jose.JOSEException;
import com.nimbusds.jose.JWSAlgorithm;
import com.nimbusds.jose.JWSHeader;
import com.nimbusds.jose.JWSSigner;
import com.nimbusds.jose.crypto.MACSigner;
import com.nimbusds.jwt.JWTClaimsSet;
import com.nimbusds.jwt.SignedJWT;

public class JWT {

    public static void main(String[] args) {

        String jwt = generateJWT();
        System.out.println("Generating JWT...");
        System.out.println(jwt);
    }

    public static String generateJWT(){
        // Replace the example values below (remove the brackets).
        // Store secrets securely based on your team's best practices.
        // See: https://help.tableau.com/current/online/en-us/connected_apps_direct.htm

        String secretId = "[Tableau Connected App Direct Trust Secret ID]";
        String secretValue = "[Tableau Connected App Direct Trust Secret Value]";
        String clientId = "[Tableau Connected App Direct Trust Client ID]";
        String username = "[Tableau Username]";
        double tokenExpiryInMinutes = 1; // Max of 10 minutes.
        Calendar expiry = Calendar.getInstance();
        expiry.add(Calendar.MINUTE, (int)tokenExpiryInMinutes);

        // Remove 'tableau:views:embed_authoring' scope if Authoring is not needed.
        // Remove 'tableau:insights:embed' scope if Pulse is not needed.
        String[] scopes = {"tableau:views:embed", "tableau:views:embed_authoring", "tableau:insights:embed" };

        UUID myGeneratedId = UUID.randomUUID();

        // JWT generation: any strings defined below here are hardcoded special values
        // once built, it is immutable
        JWTClaimsSet claimsSet = new JWTClaimsSet.Builder()
        .audience("tableau")
        .subject(username)
        .expirationTime(expiry.getTime())
        .issuer(clientId)
        .issueTime(new Date())
        .jwtID(myGeneratedId.toString())
        .claim("scp", scopes)
        //  User attributes are optional.
        //  Add additional claims if desired.
        //  Example:
        //  .claim("Region", "East")
        .build();

        String JWTValue;
        try {
            JWSSigner signer = new MACSigner(secretValue);
            JWSHeader header = new JWSHeader.Builder(JWSAlgorithm.HS256).keyID(secretId).customParam("iss", clientId).build();

            SignedJWT signedJWT = new SignedJWT(header, claimsSet);
            signedJWT.sign(signer);
            JWTValue = signedJWT.serialize();
        } catch (JOSEException jEx) {
            System.out.println("JWT signing failed: ");
            System.out.println(jEx);
            JWTValue = jEx.getCause().getClass().getSimpleName();
        }
        return JWTValue;
    }

}