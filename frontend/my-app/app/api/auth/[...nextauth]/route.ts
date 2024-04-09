
import DuendeIDS6Provider from "next-auth/providers/duende-identity-server6"
import NextAuth, { NextAuthOptions } from "next-auth"

export const authOptions: NextAuthOptions = {
    session: {
        strategy: 'jwt'
    },
    providers: [
        DuendeIDS6Provider({
            id: 'id-server',
            clientId: 'nextApp',
            clientSecret: 'secret',
            issuer: 'http://localhost:5000',
            authorization: {params: {scope: 'openid profile auctionApp'}},
            idToken: true
        })
    ],
    callbacks: {
        async jwt({token, profile, account}){
            if(profile) {
                token.username = profile.username
            }
            if(account) {
                token.access_token = account.access_token;
            }
            console.log({token, profile});
            return token;
        },
        async session({ session, token }){
            if(token) {
                session.user.username = token.username;
            }
            return session;
        }
    }
}

const handler = NextAuth(authOptions);
export { handler as GET, handler as POST }