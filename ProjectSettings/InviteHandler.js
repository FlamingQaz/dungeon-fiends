const {
    Client,
    Intents
} = require('discord.js')

const client = new Client({
    intents: [Intents.FLAGS.GUILDS, Intents.FLAGS.GUILD_MEMBERS, Intents.FLAGS.GUILD_BANS, Intents.FLAGS.GUILD_EMOJIS_AND_STICKERS, Intents.FLAGS.GUILD_INTEGRATIONS, Intents.FLAGS.GUILD_WEBHOOKS, Intents.FLAGS.GUILD_INVITES, Intents.FLAGS.GUILD_VOICE_STATES, Intents.FLAGS.GUILD_PRESENCES, Intents.FLAGS.GUILD_MESSAGES, Intents.FLAGS.GUILD_MESSAGE_REACTIONS, Intents.FLAGS.GUILD_MESSAGE_TYPING, Intents.FLAGS.DIRECT_MESSAGES, Intents.FLAGS.DIRECT_MESSAGE_REACTIONS, Intents.FLAGS.DIRECT_MESSAGE_TYPING],
    partials: ['MESSAGE', 'CHANNEL', 'REACTION']
})
const guildInvites = new Map()
client.on('inviteCreate', async invite => {
    console.log("INVITE CREATE RUN");
    const invites = await invite.guild.invites.fetch();

    const codeUses = new Map();
    invites.each(inv => codeUses.set(inv.code, inv.uses));

    guildInvites.set(invite.guild.id, codeUses);
})

client.once('ready', () => {
    client.guilds.cache.forEach(guild => {
        guild.invites.fetch()
            .then(invites => {
                console.log("INVITES CACHED");
                const codeUses = new Map();
                invites.each(inv => codeUses.set(inv.code, inv.uses));

                guildInvites.set(guild.id, codeUses);
            })
            .catch(err => {
                console.log("OnReady Error:", err)
            })
    })
})

client.on('guildMemberAdd', async member => {
    const cachedInvites = guildInvites.get(member.guild.id)
    const newInvites = await member.guild.invites.fetch();
    try {
        const usedInvite = newInvites.find(inv => {
            console.log(cachedInvites.get(inv.code) ? "Invite is cached" : "Invite is not cached");
            console.log("Cached uses:", cachedInvites.get(inv.code));
            console.log("New uses:", inv.uses);
            return cachedInvites.get(inv.code) < inv.uses;
        });
        console.log("Cached", [...cachedInvites.keys()])
        console.log("New", [...newInvites.values()].map(inv => inv.code))
        console.log("Used", usedInvite)
        console.log(`The code ${usedInvite.code} was just used by ${member.user.username}.`)
    } catch (err) {
        console.log("OnGuildMemberAdd Error:", err)
    }

    newInvites.each(inv => cachedInvites.set(inv.code, inv.uses));
    guildInvites.set(member.guild.id, cachedInvites);
})